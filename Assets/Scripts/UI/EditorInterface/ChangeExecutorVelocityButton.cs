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
        ExecuterVelocity currentVelocity;

        void Start() {
            currentVelocity = Executer.instance.velocity;
            icon.SetState(GetIcon());

            button.AddListener(() => {
                currentVelocity = NextVelocity();
                icon.SetState(GetIcon());

                Executer.instance.SetVelocity(currentVelocity);
            });

        }

        ExecuterVelocity NextVelocity() {
            if (currentVelocity == ExecuterVelocity.Immediately) {
                return ExecuterVelocity.EverySecond;
            } else if (currentVelocity == ExecuterVelocity.EverySecond) {
                return ExecuterVelocity.EveryThreeSeconds;
            } else {
                return ExecuterVelocity.Immediately;
            }
        }

        string GetIcon() {
            if (currentVelocity == ExecuterVelocity.Immediately) {
                return $"{ExecuterVelocity.Immediately}";
            } else if (currentVelocity == ExecuterVelocity.EverySecond) {
                return $"{ExecuterVelocity.EverySecond}";
            } else {
                return $"{ExecuterVelocity.EveryThreeSeconds}";
            }
        }

    }
}