// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public class TutorialLevel22 : Tutorial {

        // Internal types
        enum State {
            Completed,
            Started,
            Presenting,
            PresentingPanel,
            PresentingRoute,
            PresentingSorting,
            Final
        }

        // State variables
        State currentState;

        // Other variables
        List<(State state, string message)> initialMessages = new List<(State state, string message)> {
            (state: State.Presenting,
                message: "Este nivel es más complicado."),
            (state: State.PresentingPanel,
                message: "Necesitas que el robot ingrese los números correctos en un panel de la parte superior."),
            (state: State.PresentingRoute,
                message: "El código correcto se genera aleatoriamente cuando Copper cruza la puerta."),
            (state: State.PresentingSorting,
                message: "Tienes que escanear esos números y ordenarlos de menor a mayor para que la puerta de salida se abra."),
            (state: State.Final,
                message: "Te retamos a que no uses el nodo de ordenar arreglo (Pero no es necesario). Buena suerte"),
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
                type: MessageType.Normal,
                hideInNewMessage: false,
                seconds: 10,
                onFullShowCallback: () => InitialMessages(messageIndex + 1)
            );
        }
    }
}