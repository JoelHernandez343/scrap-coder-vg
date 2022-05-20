// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Tutorial;
using ScrapCoder.GameInput;

namespace ScrapCoder.Player {
    public class TutorialMovement : MonoBehaviour {

        // Internal types
        enum Directions { Up, Down, Left, Right }

        // State variables
        List<bool> movementRegister = new List<bool> { false, false, false, false };
        bool signalSent = false;

        // Methods
        void FixedUpdate() {
            Movement();
        }

        void Movement() {
            if (signalSent) return;

            var horizontalMovement = InputController.instance.GetAxisRaw("Horizontal");
            var verticalMovement = InputController.instance.GetAxisRaw("Vertical");

            if (!movementRegister[(int)Directions.Up]) {
                movementRegister[(int)Directions.Up] = verticalMovement > 0;
            }

            if (!movementRegister[(int)Directions.Down]) {
                movementRegister[(int)Directions.Down] = verticalMovement < 0;
            }

            if (!movementRegister[(int)Directions.Right]) {
                movementRegister[(int)Directions.Right] = horizontalMovement > 0;
            }

            if (!movementRegister[(int)Directions.Left]) {
                movementRegister[(int)Directions.Left] = horizontalMovement < 0;
            }

            MovementSignal();
        }

        void MovementSignal() {
            var allDirections = true;
            movementRegister.ForEach(r => allDirections &= r);

            if (allDirections) {
                TutorialController.instance.ReceiveSignal(signal: "movementCompleted");
                signalSent = true;
            }
        }

    }
}