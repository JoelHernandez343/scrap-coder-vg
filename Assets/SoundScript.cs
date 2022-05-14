using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour {
    // Editor variables
    [SerializeField] private AudioClip[] audios;

    // Lazy variables
    AudioSource _source;
    AudioSource source => _source ??= GetComponent<AudioSource>() as AudioSource;

    // Methods

    public void Walk() {
        source.clip = audios[Random.Range(0, audios.Length)];
        source.Play();
    }
}
