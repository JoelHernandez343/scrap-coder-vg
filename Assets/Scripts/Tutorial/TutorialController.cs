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

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void StartTutorial() {
            tutorial.StartTutorial();
        }


    }
}