// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class texttmp : MonoBehaviour {

        // Editor variables
        [SerializeField] string message;
        [SerializeField] MessageType type;

        // Methods
        void Start() {

            StartCoroutine(Wait());

        }

        IEnumerator Wait() {
            yield return new WaitForSeconds(3);

            MessagesController.instance.AddMessage(
                message: message,
                type: type
            );
        }

    }
}