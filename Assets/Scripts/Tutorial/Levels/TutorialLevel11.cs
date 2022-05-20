// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialLevel11 : Tutorial {

        // Internal types
        enum State {
            Completed,
            Started,
            Movement,
            WaitingForMovement,
            InteractWithEditor,
            WaitingForInteract,
            PutBeginNode,
            WaitingForBeginNode,
            PutEndNode,
            WaitingForEndNode,
            PutFourWalksNodes,
            WaitingForWalks,
        }

        // Editor variables
        [SerializeField] Sprite wasdSprite;
        [SerializeField] Sprite eSprite;

        // State variables
        State currentState;

        // Methods
        protected override void CustomStartTutorial() {
            currentState = State.Started;

            StartCoroutine(ShowMovementMessage());
        }

        IEnumerator ShowMovementMessage() {
            yield return new WaitForSeconds(2);

            currentState = State.Movement;

            ShowMessage(
                message: "Puedes moverte con las teclas W A S D",
                type: MessageType.Normal,
                customSprite: wasdSprite,
                onFullShowCallback: () => currentState = State.WaitingForMovement
            );
        }

        public override bool ReceiveSignal(string signal) {
            Debug.Log($"Received {signal}");
            if (signal == "movementCompleted" && currentState == State.WaitingForMovement) {
                HideMessageOfCurrentState();
                return true;
            }

            return false;
        }

    }
}