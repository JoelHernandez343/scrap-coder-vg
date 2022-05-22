// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Tutorial;

namespace ScrapCoder.UI {
    public class SplashScreenTutorialCb : MonoBehaviour {

        // Lazy variables
        SplashScreenDissolver _splashScreen;
        SplashScreenDissolver splashScreen => _splashScreen ??= (GetComponent<SplashScreenDissolver>() as SplashScreenDissolver);

        // Methods
        void Start() {
            splashScreen.AddListener(
                listener: () => TutorialController.instance.StartTutorial(),
                eventType: SplashScreenEvent.OnDissapear
            );
        }

    }
}