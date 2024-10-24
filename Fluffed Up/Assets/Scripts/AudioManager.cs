using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public Sound3D sound3DPrefab;
    public WaveManager waveManagerScript;

    private UnityAction<Vector3, AudioClip> playerSoundEffectEventListener;

    // Unity Monobehaviour
    void Awake()
    {
        playerSoundEffectEventListener = new UnityAction<Vector3, AudioClip>(playerSoundEventHandler);
    }

    // Unity Monobehaviour
    void OnEnable()
    {
        // Start Listening to event
        waveManagerScript.playerSoundEvent.AddListener(playerSoundEffectEventListener);
    }

    void OnDisable()
    {
        // Stop listening to event
        waveManagerScript.playerSoundEvent.RemoveAllListeners();
    }

    void playerSoundEventHandler(Vector3 worldPos, AudioClip audioClip)
    {
        if (sound3DPrefab)
        {
            Sound3D sound3DObject = Instantiate(sound3DPrefab, worldPos, Quaternion.identity, null);
            sound3DObject.audioSrc.clip = audioClip;

            sound3DObject.audioSrc.minDistance = 5f;
            sound3DObject.audioSrc.maxDistance = 100f;

            sound3DObject.audioSrc.Play();
        }
    }
}
