using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    private Animator anim;
    [SerializeField]
    private Animator blinkTemplate;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartGlow()
    {
        anim.SetBool("Glowing", true);
    }

    public void EndGlow()
    {
        anim.SetBool("Glowing", false);
    }

    public void Blink()
    {
        Animator blink = Instantiate(blinkTemplate,transform.position,transform.rotation,transform);
        Destroy(blink.gameObject, 1);
    }
}
