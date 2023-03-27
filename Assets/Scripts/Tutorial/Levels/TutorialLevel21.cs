// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialLevel21 : Tutorial {

        // Internal types
        enum State {
            Completed,
            Started,
            Presenting,
            PresentingRoute,
            PresentingVariable,
            Final
        }

        // State variables
        State currentState;

        // Other variables
        List<(State state, string message)> initialMessages = new List<(State state, string message)> {
            (state: State.Presenting,
                message: "Este nivel es más complicado."),
            (state: State.PresentingRoute,
                message: "Tienes que hacer que el robot camine en la espiral, cuya primera curva consta de 10 pasos. Y la segunda de 9..."),
            (state: State.PresentingVariable,
                message: "Este recorrido es más fácil usando una variable decreciente."),
            (state: State.Final,
                message: "Buena suerte."),
        };

        // Methods
        protected override void CustomStartTutorial() {
            currentState = State.Started;

            StartCoroutine(ShowInitialMessage());
        }

        IEnumerator ShowInitialMessage() {
            yield return new WaitForSeconds(2);

            InitialMessages(messageIndex: 0);
        }

        public override bool ReceiveSignal(string signal) {
            return false;
        }

        public void InitialMessages(int messageIndex) {
            if (messageIndex == initialMessages.Count) {
                return;
            }
            var (state, message) = initialMessages[messageIndex];

            currentState = state;

            ShowMessage(
                message: message,
                type: MessageStatus.Normal,
                hideInNewMessage: false,
                seconds: 10,
                onFullShowCallback: () => InitialMessages(messageIndex + 1)
            );
        }
    }
}