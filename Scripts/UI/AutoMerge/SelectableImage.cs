using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableImage : MonoBehaviour
{
    [SerializeField]
    private Color selectedColor;

    private Color startingColor;
    private void Awake()
    {
        startingColor = GetComponent<Image>().color;
    }

    public void Activate()
    {
        GetComponent<Image>().color = selectedColor;
    }

    public void DeActivate()
    {
        GetComponent<Image>().color = startingColor;
    }
}
