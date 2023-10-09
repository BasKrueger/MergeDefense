using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMerge : MonoBehaviour
{
    private SelectableImage icon;
    private bool merging = false;

    private void Awake()
    {
        icon = GetComponentInChildren<SelectableImage>();
        this.enabled = false;
    }

    public void OnClick()
    {
        this.enabled = !enabled;
        if (this.enabled)
        {
            icon.Activate();
        }
        else
        {
            icon.DeActivate();
        }
    }

    private void Update()
    {
        if (merging)
        {
            return;
        }

        (TurretTile A, TurretTile B) pair = GetMergePair();
        if(pair.A != null && pair.B != null)
        {
            StartCoroutine(Merge(pair.A, pair.B));
        }
    }

    private IEnumerator Merge(TurretTile A, TurretTile B)
    {
        SetMerging(true);

        const float mergeSpeed = 10f;
        float activeMergeTimer = 0f;

        yield return new WaitForSeconds(0.1f);

        Vector3 startPoint = A.turret.transform.position;
        Vector3 endPoint = B.turret.transform.position;
        float mergeTime = Vector3.Distance(startPoint, endPoint) / mergeSpeed;

        while(activeMergeTimer < mergeTime)
        {
            activeMergeTimer += Time.deltaTime;
            A.turret.transform.position = Vector3.Lerp(startPoint, endPoint, activeMergeTimer / mergeTime);
            yield return new WaitForEndOfFrame();
        }

        B.turret.Merge(A.turret);
        A.turret = null;

        yield return new WaitForSeconds(0.1f);

        SetMerging(false);

        void SetMerging(bool state)
        {
            merging = state;
            A.lockDragDrop = state;
            B.lockDragDrop = state;
            GetComponentInChildren<Animator>().SetBool("Spinning", state);
        }
    }

    private (TurretTile, TurretTile) GetMergePair()
    {
        MergeTile[] mergeTiles = FindObjectsOfType<MergeTile>();
        BuildTile[] buildTiles = FindObjectsOfType<BuildTile>();

        foreach(MergeTile a in mergeTiles)
        {
            if(a.turret == null)
            {
                continue;
            }

            foreach(MergeTile b in mergeTiles)
            {
                if (b.turret == null)
                {
                    continue;
                }

                if (a.turret.CanMerge(b.turret))
                {
                    return (a, b);
                }
            }
            foreach(BuildTile b in buildTiles)
            {
                if (b.turret == null)
                {
                    continue;
                }

                if (a.turret.CanMerge(b.turret))
                {
                    return (a, b);
                }
            }
        }

        return (null, null);
    }
}
