
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource EffectSource;
    public AudioSource MusicSource;
    public AudioClip[] BackGroundClips;
    public int CurrectClip = -1;

    public static SoundManager Instance = null;

    public float LowPitchRange = 0.95f;
    public float HighPitchRange = 1.05f;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            ChangeBackgroundAudio();
        } else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    } 

    public void ChangeBackgroundAudio()
    {
        CurrectClip = (CurrectClip + 1) % BackGroundClips.Length;
        MusicSource.clip = BackGroundClips[CurrectClip];
        MusicSource.Play();
        Invoke("ChangeAudio", BackGroundClips[CurrectClip].length);
    }

    public void PlaySingle(AudioClip clip)
    {
        EffectSource.clip = clip;
        EffectSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectSource.pitch = randomPitch;
        EffectSource.clip = clips[randomIndex];
        EffectSource.Play();
    }

}