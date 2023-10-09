using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    [HideInInspector]
    public Glow glow;
    [SerializeField]
    private TextMeshProUGUI text;

    private const float countSpeed = 40f;

    private float targetValue = 0;
    private float activeTimer = 0;

    private void Awake()
    {
        glow = GetComponentInChildren<Glow>();
    }


    public void UpdateCount(float value)
    {
        targetValue = value;
    }

    private void Update()
    {
        int currentValue = int.Parse(text.text);

        if (activeTimer <= 0 && currentValue != targetValue)
        {
            if (currentValue > targetValue)
            {
                currentValue--;
            }
            else
            {
                currentValue++;
            }

            text.text = currentValue.ToString();
            activeTimer = 1 / countSpeed;
        }

        activeTimer -= Time.deltaTime;
      
    }

}
