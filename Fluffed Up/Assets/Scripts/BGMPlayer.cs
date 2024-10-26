using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private AudioSource mAudioSource;
    private AudioLowPassFilter mLowPassFilter;
    public float mOriginalVolume;
    public float mDimmedVolume;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        mAudioSource = GetComponent<AudioSource>();
        mAudioSource.loop = true;
        mLowPassFilter = GetComponent<AudioLowPassFilter>();
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
