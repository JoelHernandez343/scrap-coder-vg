// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public abstract class Tutorial : MonoBehaviour {

        System.Guid currentGuid;

        protected void ShowMessage(string message, MessageType type, Sprite customSprite = null) {
            currentGuid = MessagesController.instance.AddMessage(
                message: message,
                type: type,
                isFinite: false,
                customSprite: customSprite,
                hideInNewMessage: true
            );
        }

        protected void HideMessageOfCurrentState() {
            MessagesController.instance.HideMessageWithGuid(guid: currentGuid);
        }

        abstract public void StartTutorial();

        abstract public void ReceiveSignal(string signal);

    }
}