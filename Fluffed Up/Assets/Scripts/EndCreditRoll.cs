using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndCreditRoll : MonoBehaviour
{
    public float scrollSpeed = 50f;
    public int initialHoldDuration;
    private int initialHoldCount = 0;
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialHoldCount++ > initialHoldDuration)
            transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
        else
        {
            float alpha = (float)(initialHoldCount > initialHoldDuration ? initialHoldDuration : initialHoldCount)/(float)initialHoldDuration;
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
    }
}
