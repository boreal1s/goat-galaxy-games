using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: CS4455_M1_Support
[RequireComponent(typeof(AudioSource))]
public class Sound3D : MonoBehaviour
{
    public AudioSource audioSrc;

    // Start is called before the first frame update
    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSrc.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
