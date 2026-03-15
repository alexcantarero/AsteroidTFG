
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource soundFXObj;
    private AudioSource loopingSource;

    public bool isPlaying => IsLooping;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySFX(AudioClip audioClip, float volume)
    {
        if (audioClip == null || soundFXObj == null)
        {
            Debug.LogWarning("SFXManager.PlaySFX: missing audioClip or soundFXObj prefab.");
            return;
        }

        AudioSource source = Instantiate(soundFXObj, transform.position, Quaternion.identity);
        source.clip = audioClip;
        source.volume = volume;
        source.loop = false;
        source.Play();
        float duration = source.clip.length;
        Destroy(source.gameObject, duration);
    }

    public void PlayLoopingSFX(AudioClip audioClip, float volume)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("SFXManager.PlayLoopingSFX: audioClip is null.");
            return;
        }

        // If we already have a looping source playing the same clip, do nothing
        if (IsLooping && loopingSource.clip == audioClip && loopingSource.isPlaying) return;

        // Create looping source if needed
        if (loopingSource == null)
        {
            if (soundFXObj != null)
            {
                loopingSource = Instantiate(soundFXObj, transform);
            }
            else
            {
                loopingSource = gameObject.AddComponent<AudioSource>();
            }

            loopingSource.loop = true;
        }

        loopingSource.clip = audioClip;
        loopingSource.volume = volume;
        loopingSource.loop = true;
        loopingSource.Play();
    }

    public void StopLoopingSFX()
    {
        if (loopingSource != null && loopingSource.isPlaying)
        {
            loopingSource.Stop();
        }
    }

    public bool IsLooping => loopingSource != null && loopingSource.isPlaying;
}