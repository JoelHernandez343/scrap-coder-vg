using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Audio {
    public class SoundScript : MonoBehaviour {
        // Editor variables
        [SerializeField] List<AudioClip> audios;

        // Lazy variables
        AudioSource _source;
        AudioSource source => _source ??= GetComponent<AudioSource>() as AudioSource;

        // Methods
        public void Walk() {
            PlayClip();
        }

        public void PlayClip(bool random = true) {
            source.clip = audios[Random.Range(0, audios.Count)];
            source.Play();
        }
    }
}