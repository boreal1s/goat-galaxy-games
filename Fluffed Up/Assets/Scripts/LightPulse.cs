using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public float intensityMax;
    public float intensityMin;
    public float pulsePeriod;
    public bool lightActivate;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (lightActivate == true)
        {
            GetComponent<Light>().intensity = intensityMin + (Mathf.Sin(Time.time*2*Mathf.PI/pulsePeriod) + 1)*(intensityMax - intensityMin)/2;
        }
        else
        {
            GetComponent<Light>().intensity = 0;
        }
    }
}
