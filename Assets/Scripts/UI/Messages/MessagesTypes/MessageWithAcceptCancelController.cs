// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class MessageWithAcceptCancelController : MonoBehaviour {

        [SerializeField] ButtonController acceptButton;
        [SerializeField] ButtonController cancelButton;

        void Start() {
            acceptButton.AddListener(() => MessagesController.instance.HideCurrentMessage());
            cancelButton.AddListener(() => MessagesController.instance.HideCurrentMessage());
        }
    }
}