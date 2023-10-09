using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretUILevel : MonoBehaviour
{
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();    
    }

    public void UpdateValue(float value)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
    }

    public void UpdatePosition(Transform t1, RectTransform canvas)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(t1.position);
        Vector2 p1 = new Vector2(
        ((ViewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));

        rect.anchoredPosition = p1;
    }
}
