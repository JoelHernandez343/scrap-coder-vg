// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialLevel11 : Tutorial {

        // Internal types
        enum State { Completed, Movement, InteractWithEditor, PutBeginNode, PutEndNode, PutFourWalksNodes }

        // Editor variables
        [SerializeField] Sprite wasdSprite;
        [SerializeField] Sprite eSprite;

        // State variables
        State currentState;

        // Methods
        public override void StartTutorial() {
            currentState = State.Movement;

            StartCoroutine(ShowMovementMessage());

            Debug.Log("This is the tutorial 1 1");
        }

        IEnumerator ShowMovementMessage() {
            yield return new WaitForSeconds(3);

            ShowMessage(
                message: "Puedes moverte con las teclas W A S D",
                type: MessageType.Normal,
                customSprite: wasdSprite
            );
        }

        public override void ReceiveSignal(string signal) {
            if (signal == "movementCompleted" && currentState == State.Movement) {
                HideMessageOfCurrentState();
            }
        }

    }
}