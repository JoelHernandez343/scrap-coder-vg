// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class MessageContainer : MonoBehaviour {

        // Editor variables
        // [SerializeField] ExpandableText titleText;
        [SerializeField] ExpandableText messageText;
        [SerializeField] NodeSprite icon;
        [SerializeField] NodeSprite customSprite;

        [SerializeField] ButtonController discardButton;

        [SerializeField] NodeTransform normalMessageParent;
        [SerializeField] NodeTransform spriteMessageParent;

        // Methods

        public void SetDiscardCallback(System.Action discardCallback) {
            discardButton.AddListener(listener: discardCallback);
        }

        public void ShowNewMessage(MessageInfo message) {

            if (message.customSprite != null) {
                ShowCustomSprite(message);
            } else {
                ShowNormal(message);
            }

        }

        void ShowCustomSprite(MessageInfo message) {

        }

        void ShowNormal(MessageInfo message) {

        }

        public void Hide() {

        }

    }
}