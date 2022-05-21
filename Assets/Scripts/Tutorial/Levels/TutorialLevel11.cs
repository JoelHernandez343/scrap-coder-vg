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
            InspectOn,
            WaitingForInspectOn,
            InspectOff,
            WaitingForInspectOff,
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
        [SerializeField] Sprite qSprite;
        [SerializeField] Sprite rSprite;

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
            Debug.Log($"Signal received: {signal}");

            if (signal == "movementCompleted" && currentState == State.WaitingForMovement) {
                HideMessageOfCurrentState();
                InspectOn();
                return true;
            }

            if (signal == "inspectOn" && currentState == State.WaitingForInspectOn) {
                HideMessageOfCurrentState();
                InspectOff();
                return true;
            }

            if (signal == "inspectOff" && currentState == State.WaitingForInspectOff) {
                HideMessageOfCurrentState();
                InteractWithEditor();
                return true;
            }

            if (signal == "editorToggled" && currentState == State.WaitingForInteract) {
                HideMessageOfCurrentState();
                PutBeginNode();
                return true;
            }

            if (signal == "placedTypeBegin" && currentState == State.WaitingForBeginNode) {
                HideMessageOfCurrentState();
                return true;
            }

            return false;
        }

        void InspectOn() {
            currentState = State.InspectOn;

            ShowMessage(
                message: "Con la tecla Q puedes observar el botón que abre la puerta y cómo este se conecta.",
                type: MessageType.Normal,
                customSprite: qSprite,
                onFullShowCallback: () => currentState = State.WaitingForInspectOn
            );
        }

        void InspectOff() {
            currentState = State.InspectOff;

            ShowMessage(
                message: "Presiona Q para cerrar este modo de inspección.",
                type: MessageType.Normal,
                customSprite: qSprite,
                onFullShowCallback: () => currentState = State.WaitingForInspectOff
            );
        }

        void InteractWithEditor() {
            currentState = State.InteractWithEditor;

            ShowMessage(
                message: "Acércate al robot y presiona E para abrir el editor del robot.",
                type: MessageType.Normal,
                customSprite: eSprite,
                onFullShowCallback: () => currentState = State.WaitingForInteract
            );
        }

        void PutBeginNode() {
            currentState = State.PutBeginNode;

            ShowMessage(
                message: "Para programar al robot, necesitas decirle cuando comience. Para eso agrega un nodo de Inicio. Lo puedes encontrar en el primer menú en la esquina superior izquierda.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForBeginNode
            );
        }

    }
}