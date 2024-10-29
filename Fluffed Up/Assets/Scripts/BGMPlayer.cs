using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;
    private AudioSource mAudioSource;
    private AudioLowPassFilter mLowPassFilter;
    public float mOriginalVolume;
    public float mDimmedVolume;

     private void Awake()
    {
        // Check if an instance of BGMPlayer already exists
        if (instance != null && instance != this)
        {
            // Destroy this game object if a duplicate is found
            Destroy(gameObject);
            return;
        }

        // Set this as the instance and make it persistent across scenes
        instance = this;
        DontDestroyOnLoad(gameObject);

        mAudioSource = GetComponent<AudioSource>();
        mAudioSource.loop = true;
        mLowPassFilter = GetComponent<AudioLowPassFilter>();

        if (!mAudioSource.isPlaying)
        {
            mAudioSource.Play();
        }
    }

    public void DimAndDull()
    {
        mAudioSource.volume = mDimmedVolume;
        mLowPassFilter.cutoffFrequency = 480;
    }

    public void LoudAndClear()
    {
        mAudioSource.volume = mOriginalVolume;
        mLowPassFilter.cutoffFrequency = 22000;
    }
}
