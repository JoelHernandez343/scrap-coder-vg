// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialController : MonoBehaviour {

        // Static variables
        public static TutorialController instance;

        // Editor variables
        [SerializeField] Tutorial tutorial;

        // Lazy variables
        Tutorial realTutorial => tutorial != null ? tutorial : null;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void StartTutorial() {
            realTutorial?.StartTutorial();
        }

        public bool ReceiveSignal(string signal) {
            return realTutorial?.isStarted == true
                ? realTutorial.ReceiveSignal(signal)
                : false;
        }

    }
}