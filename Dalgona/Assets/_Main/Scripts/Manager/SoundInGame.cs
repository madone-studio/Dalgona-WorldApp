using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundInGame : MonoBehaviour
{
    public AudioClip[] audioClips;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = audioClips[Random.Range(0, audioClips.Length - 1)];
        audioSource.loop = true;
        audioSource.volume = Module.musicFx;
        audioSource.Play();
    }

    private void OnEnable()
    {
        Module.Event_ChangeMusic += Module_Event_ChangeMusic;
    }

    private void Module_Event_ChangeMusic()
    {
        audioSource.volume = Module.musicFx;
        audioSource.Play();
    }

    private void OnDisable()
    {
        Module.Event_ChangeMusic -= Module_Event_ChangeMusic;
    }


}
