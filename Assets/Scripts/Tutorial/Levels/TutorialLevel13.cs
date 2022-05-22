// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialLevel13 : Tutorial {

        // Internal types
        enum State {
            Completed,
            Started,
            PresentingLevel,
            PresentingRepeat,
            PresentingCondition,
            PresentingTrueConstant,
            HowToResolve,
            PutRepeat,
            WaitingForRepeat,
            PutTrueConstant,
            WaitingForTrueConstant,
            FinalQuestion
        }

        // State variables
        State currentState;
        bool placedTypeBegin;
        bool placedTypeEnd;
        bool placedTypeRepeat;

        // Other variables
        List<(State state, string message)> initialMessages = new List<(State state, string message)> {
            (state: State.PresentingLevel,
                message: "Ahora te presentamos la instrucción Repetir si."),
            (state: State.PresentingRepeat,
                message: "La instrucción Repetir si te permite ejecutar instrucciones mientras una condición sea verdadera."),
            (state: State.PresentingCondition,
                message: "Una condición podemos verla como una pregunta que responde Sí o No, o Verdadero o Falso. Podemos tener preguntas que siempre te devuelvan Sí"),
            (state: State.PresentingTrueConstant,
                message: "Un ejemplo de esas condiciones que siempre devuelven Sí es la constante Verdadero, representada por un foco siempre encendido."),
            (state: State.HowToResolve,
                message: "Este nivel se resuelve más fácilmente utilizando un Repetir Si: Verdadero, un ciclo infinito."),
        };

        // Methods
        protected override void CustomStartTutorial() {
            currentState = State.Started;

            placedTypeBegin = false;
            placedTypeEnd = false;
            placedTypeRepeat = false;

            StartCoroutine(ShowInitialMessage());
        }

        IEnumerator ShowInitialMessage() {
            yield return new WaitForSeconds(2);

            InitialMessages(messageIndex: 0);
        }

        public override bool ReceiveSignal(string signal) {
            Debug.Log($"[Tutorial 1-3] Signal received: {signal}");

            if (
                (signal == "placedTypeBegin" || signal == "placedTypeEnd" || signal == "placedTypeRepeat") &&
                currentState == State.WaitingForRepeat
            ) {
                placedTypeBegin = placedTypeBegin
                    ? placedTypeBegin
                    : signal == "placedTypeBegin";

                placedTypeEnd = placedTypeEnd
                    ? placedTypeEnd
                    : signal == "placedTypeEnd";

                placedTypeRepeat = placedTypeRepeat
                    ? placedTypeRepeat
                    : signal == "placedTypeRepeat";

                if (placedTypeBegin && placedTypeEnd && placedTypeRepeat) {
                    HideMessageOfCurrentState();
                    PutTrueConstant();
                }

                return true;
            }

            if (signal == "placedTypeTrueConstant" && currentState == State.WaitingForTrueConstant) {
                HideMessageOfCurrentState();
                FinalQuestion();
                return true;
            }

            return false;
        }

        public void InitialMessages(int messageIndex) {
            if (messageIndex == initialMessages.Count) {
                PutRepeat();
                return;
            }
            var (state, message) = initialMessages[messageIndex];

            currentState = state;

            ShowMessage(
                message: message,
                type: MessageType.Normal,
                hideInNewMessage: false,
                seconds: 10,
                onFullShowCallback: () => InitialMessages(messageIndex + 1)
            );
        }

        void PutRepeat() {
            currentState = State.PutRepeat;

            ShowMessage(
                message: "Ahora, abre el editor y coloca un Inicio, un Fin y un Repetir Si y conéctalos entre sí.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForRepeat
            );
        }

        void PutTrueConstant() {
            currentState = State.PutTrueConstant;

            ShowMessage(
                message: "Perfecto, ahora coloca una constante Verdadera del menú Condiciones en el nodo Repetir si.",
                type: MessageType.Normal,
                onFullShowCallback: () => currentState = State.WaitingForTrueConstant
            );
        }

        void FinalQuestion() {
            currentState = State.FinalQuestion;

            ShowMessage(
                message: "Ahora te toca a ti, ¿Qué instrucción se tiene que repetir siempre para que el robot pueda abrir las puertas mientras tú caminas?",
                type: MessageType.Normal,
                seconds: 10,
                onFullShowCallback: () => currentState = State.Completed
            );
        }
    }
}