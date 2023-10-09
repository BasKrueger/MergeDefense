using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PreviewTile : TurretTile
{
    public IEnumerator DeleteActiveShowcase()
    {
        if (base.turret != null)
        {
            turret.gameObject.SetActive(false);
            Destroy(turret.gameObject, 4);
            yield return StartCoroutine(turret.Dissolve(0.1f));
            base.turret = null;
        }
    }

    public void ShowcaseTurret(Turret turretTemplate)
    {
        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return StartCoroutine(DeleteActiveShowcase());

            Turret copy = Instantiate(turretTemplate);
            base.RegisterTurret(copy);

            yield return StartCoroutine(copy.Manifest(0.25f, 0.05f, true));
        }
    }
}
