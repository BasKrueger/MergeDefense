using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyMesh : MonoBehaviour
{
    [SerializeField]
    private float turnSpeed;

    private Vector3 target;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (target == new Vector3() || Vector3.Angle(target - transform.position, transform.forward) <= 1)
        {
            target = new Vector3();
            return;
        }

        Vector3 direction = target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (turnSpeed * 10) * Time.deltaTime);
    }

    public void TurnAt(Vector3 target)
    {
        target.y = transform.position.y;
        this.target = target;
    }

    public void PlayAnimation(EnemyAnimation animation)
    {
        switch (animation)
        {
            case EnemyAnimation.Idle:
                break;
            case EnemyAnimation.Die:
                anim.SetBool("Dead", true);
                break;
        }
    }

    public void SetRedPercent(float percent)
    {
        Color color = new Color(1, percent, percent, 1);

        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_Color", color);
            }
        }
    }

    #region animation calls
    private void Dissolve()
    {
        float dissolveTime = 1f;

        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach(Material mat in renderer.materials)
            {
                StartCoroutine(dissolve(mat));
            }
        }

        IEnumerator dissolve(Material mat)
        {
            mat.SetFloat("_DissolveWidth", 0.2f);
            float time = 0;

            while(time < dissolveTime)
            {
                mat.SetFloat("_DissolveAmount", time/dissolveTime);

                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void OnDamageFinished()
    {
        anim.SetBool("Damaged", false);
    }
    #endregion
}
