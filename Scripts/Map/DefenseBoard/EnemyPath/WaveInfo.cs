using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfo : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private Image SpawnProgress;
    [SerializeField, ChildGameObjectsOnly]
    private Image killProgress;

    private float spawnTargetValue = 0;
    private float killTargetValue = 0;

    public void Setup(EnemyWave wave, float index)
    {
        if(index < 0)
        {
            GetComponent<Animator>().Play("Dummy");
            return;
        }
        GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();

        wave.WaveStarted.AddListener(Highlight);
        wave.SpawnUpdated.AddListener(UpdateSpawnProgressValue);
        wave.KillUpdated.AddListener(UpdateKillProgressValue);
        wave.WaveFinished.AddListener(DeHighlight);
    }

    private void Update()
    {
        if(spawnTargetValue > SpawnProgress.fillAmount)
        {
            SpawnProgress.fillAmount += 1 * Time.deltaTime;
        }
        if(killTargetValue > killProgress.fillAmount)
        {
            killProgress.fillAmount += 1 * Time.deltaTime;
        }
    }

    public void Highlight()
    {
        GetComponent<Animator>().SetBool("highlighted", true);
    }

    private void DeHighlight()
    {
        GetComponent<Animator>().SetBool("highlighted", false);
    }

    private void UpdateSpawnProgressValue(float percent)
    {
        spawnTargetValue = percent;
    }

    private void UpdateKillProgressValue(float percent)
    {
        killTargetValue = percent;
    }

    public void Destroy()
    {
        GetComponent<Animator>().SetBool("active", false);
        Destroy(this.gameObject, 3);
        Destroy(this);
    }
}
