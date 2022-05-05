// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class ChangeExecutorVelocityButton : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeSprite icon;

        // Lazy variables
        ButtonController _button;
        ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        // State variables
        ExecuterTiming currentTiming;

        void Start() {
            currentTiming = Executer.instance.timing;
            icon.SetState(GetIcon());

            button.AddListener(() => {
                currentTiming = NextTiming();
                icon.SetState(GetIcon());

                Executer.instance.SetTiming(currentTiming);
            });

        }

        ExecuterTiming NextTiming() {
            if (currentTiming == ExecuterTiming.Immediately) {
                return ExecuterTiming.EverySecond;
            } else if (currentTiming == ExecuterTiming.EverySecond) {
                return ExecuterTiming.EveryThreeSeconds;
            } else {
                return ExecuterTiming.Immediately;
            }
        }

        string GetIcon() {
            if (currentTiming == ExecuterTiming.Immediately) {
                return $"{ExecuterTiming.Immediately}";
            } else if (currentTiming == ExecuterTiming.EverySecond) {
                return $"{ExecuterTiming.EverySecond}";
            } else {
                return $"{ExecuterTiming.EveryThreeSeconds}";
            }
        }

    }
}