using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    public AudioSource audioSource;
    public override void InitAwake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [Header("List audioClip UI")]
    public AudioClip[] audioClips;

    public AudioClip[] audioCookies;

    /// <summary>
    /// 0. Click - 
    /// </summary>
    /// <param name="type"></param>
    public void PlayFx(int type = 0)
    {
        audioSource.volume = Module.soundFx;
        audioSource.clip = audioClips[type];
        audioSource.Play();
    }

    public void PlayOnCamera(int type = 0)
    {
        AudioSource.PlayClipAtPoint(audioClips[type], Camera.main.transform.position, Module.soundFx);
    }

    public void PlayOnCamera(AudioClip _clip)
    {
        AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, Module.soundFx);
    }

    public void PlayCookie()
    {
        AudioClip _clip = audioCookies[Module.EasyRandom(5)];
        AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, Module.soundFx);
    }

    public bool IsFxPlaying()
    {
        return audioSource.isPlaying;
    }
}
