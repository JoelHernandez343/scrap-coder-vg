using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Audio {
    public class SoundLibrary : MonoBehaviour {

        // Editor variables
        [SerializeField] List<AudioClip> audios;
        [SerializeField] List<string> audioNames;

        // Lazy variables
        AudioSource _source;
        AudioSource source => _source ??= GetComponent<AudioSource>() as AudioSource;

        // Methods
        public void PlaySound(string sound) {
            int index = audioNames.IndexOf(sound);

            if (index != -1) {
                source.clip = audios[index];
                source.Play();
            }
        }

    }
}