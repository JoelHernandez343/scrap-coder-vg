// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialLevel12 : Tutorial {

        // Internal types
        enum State {
            Completed,
            Started,
            PresentingRotate,
            PutBeginAndEnd,
            WaitingForBeginAndEnd,
            PutRotate,
            WaitingForRotate,
            Execute,
            WaitingForExecute,
            FinalizingRotatePresentation,
            RepeatPresentation
        }

        // State variables
        State currentState;
        bool placedBegin;
        bool placedEnd;

        // Methods
        protected override void CustomStartTutorial() {
            currentState = State.Started;
            placedBegin = false;
            placedEnd = false;

            StartCoroutine(ShowInitialMessage());
        }

        IEnumerator ShowInitialMessage() {
            yield return new WaitForSeconds(2);

            PresentingRotate();
        }

        public override bool ReceiveSignal(string signal) {
            Debug.Log($"[Tutorial 1-2] Signal received: {signal}");

            if (
                (signal == "placedTypeBegin" || signal == "placedTypeEnd") &&
                currentState == State.WaitingForBeginAndEnd
            ) {
                placedBegin = placedBegin
                    ? placedBegin
                    : signal == "placedTypeBegin";

                placedEnd = placedEnd
                    ? placedEnd
                    : signal == "placedTypeEnd";

                if (placedBegin && placedEnd) {
                    HideMessageOfCurrentState();
                    PutRotate();
                }

                return true;
            }

            if (signal == "placedTypeRotate" && currentState == State.WaitingForRotate) {
                HideMessageOfCurrentState();
                ShowExecute();
                return true;
            }

            if (signal == "executerOnSuccesfully" && currentState == State.WaitingForExecute) {
                HideMessageOfCurrentState();
                FinalizingRotatePresentation();
                return true;
            }

            return false;
        }

        void PresentingRotate() {
            currentState = State.PresentingRotate;

            ShowMessage(
                message: "Ahora te presentamos la instrucción Rotar.",
                type: MessageType.Normal,
                hideInNewMessage: false,
                seconds: 3,
                onFullShowCallback: () => PutBeginAndEnd()
            );
        }

        void PutBeginAndEnd() {
            currentState = State.PutBeginAndEnd;

            ShowMessage(
                message: "Ahora, abre el editor y coloca un Inicio y un Fin.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForBeginAndEnd
            );
        }

        void PutRotate() {
            currentState = State.PutRotate;

            ShowMessage(
                message: "Perfecto, ahora coloca una instrucción Girar y conéctalo a Inicio y a Fin.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForRotate
            );
        }

        void ShowExecute() {
            currentState = State.Execute;

            ShowMessage(
                message: "Bien, asegúrate de que al girar, el robot quede mirando hacia el camino que lleva al botón. " +
                         "Ejecuta las instrucciones que acabas de poner.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForExecute
            );
        }

        void FinalizingRotatePresentation() {
            currentState = State.FinalizingRotatePresentation;

            ShowMessage(
                message: "¡Perfecto! Ahora resuelve el camino que el robot tiene que hacer " +
                         "combinando Caminar y Girar.",
                type: MessageType.Normal,
                seconds: 6,
                hideInNewMessage: false,
                onFullShowCallback: () => RepeatPresentation()
            );
        }

        void RepeatPresentation() {
            currentState = State.RepeatPresentation;

            ShowMessage(
                message: "Opcionalmente, también puedes resolver este camino con el bloque Repetir, para eso tienes que usar el nodo de Número " +
                         "y escribir cuántas veces quieres que se repita, colocando la instrucción que quieres repetir dentro de Repetir",
                type: MessageType.Normal,
                seconds: 12,
                onFullShowCallback: () => currentState = State.Completed
            );
        }
    }
}