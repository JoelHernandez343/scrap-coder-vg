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
            InitialMessage,
            Movement,
            WaitingForMovement,
            InspectOn,
            WaitingForInspectOn,
            InspectOff,
            WaitingForInspectOff,
            InteractWithEditor,
            WaitingForInteract,
            PutBeginNode,
            WaitingForBeginNode,
            PutEndNode,
            WaitingForEndNode,
            PutFourWalksNodes,
            WaitingForWalks,
            ShowExecute,
            WaitingForExecute,
            ShowOnlyReset
        }

        // Editor variables
        [SerializeField] Sprite wasdSprite;
        [SerializeField] Sprite eSprite;
        [SerializeField] Sprite qSprite;
        [SerializeField] Sprite rSprite;

        // State variables
        State currentState;
        int walkNodes;

        // Methods
        protected override void CustomStartTutorial() {
            currentState = State.Started;
            walkNodes = 0;

            StartCoroutine(ShowInitialMessage());
        }

        IEnumerator ShowInitialMessage() {
            yield return new WaitForSeconds(2);

            currentState = State.InitialMessage;

            ShowMessage(
                message: "¡Hola!, nuestro objetivo es lograr que el robot abra la puerta para que nosotros podamos salir.",
                type: MessageType.Normal,
                hideInNewMessage: false,
                seconds: 6,
                onFullShowCallback: () => ShowMovementMessage()
            );
        }

        public override bool ReceiveSignal(string signal) {
            // Debug.Log($"[Tutorial 1-1] Signal received: {signal}");

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

            if (signal == "editorOn" && currentState == State.WaitingForInteract) {
                HideMessageOfCurrentState();
                PutBeginNode();
                return true;
            }

            if (signal == "placedTypeBegin" && currentState == State.WaitingForBeginNode) {
                HideMessageOfCurrentState();
                PutEndNode();
                return true;
            }

            if (signal == "placedTypeEnd" && currentState == State.WaitingForEndNode) {
                HideMessageOfCurrentState();
                WalkNodes();
                return true;
            }

            if (signal == "placedTypeWalk" && currentState == State.WaitingForWalks) {
                walkNodes += 1;

                if (walkNodes == 4) {
                    HideMessageOfCurrentState();
                    ShowExecute();
                }

                return true;
            }

            if (signal == "executerOnSuccesfully" && currentState == State.WaitingForExecute) {
                HideMessageOfCurrentState();
                ShowReset();
                return true;
            }

            return false;
        }

        void ShowMovementMessage() {
            currentState = State.Movement;

            ShowMessage(
                message: "Puedes moverte con las teclas W A S D",
                type: MessageType.Normal,
                customSprite: wasdSprite,
                onFullShowCallback: () => currentState = State.WaitingForMovement
            );
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
                message: "Para programar al robot, necesitas decirle cuando comience. " +
                         "Para esto, agrega un nodo de Inicio. Lo puedes encontrar en el primer menú en la esquina superior izquierda. " +
                         "Arrástrarlo a la zona en blanco.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForBeginNode
            );
        }

        void PutEndNode() {
            currentState = State.PutEndNode;

            ShowMessage(
                message: "El robot también necesita saber cuándo terminar. " +
                         "Para esto, agrega un nodo de Fin. Lo puedes encontrar en el primer menú en la esquina superior izquierda. " +
                         "Arrástrarlo a la zona en blanco.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForEndNode
            );
        }

        void WalkNodes() {
            currentState = State.PutFourWalksNodes;

            ShowMessage(
                message: "¡Muy bien! Para que el robot pueda presionar el botón necesitamos que camine 4 veces. " +
                         "Coloca 4 instrucciones de Caminar y conéctalos con Inicio y al final pon el nodo Fin. " +
                         "Las puedes encontrar en el segundo menú de Instrucciones. ",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForWalks
            );
        }

        void ShowExecute() {
            currentState = State.ShowExecute;

            ShowMessage(
                message: "Para ejecutar las instrucciones que acabas de programar, dale clic al botón de ejecutar.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForExecute
            );
        }

        void ShowReset() {
            currentState = State.ShowOnlyReset;

            ShowMessage(
                message: "¡Perfecto! Si quieres reiniciar el nivel por alguna razón o porque quieres repetir estos mensajes, " +
                         "presiona R.",
                type: MessageType.Normal,
                customSprite: rSprite,
                onFullShowCallback: () => currentState = State.Completed,
                seconds: 6
            );
        }

    }
}