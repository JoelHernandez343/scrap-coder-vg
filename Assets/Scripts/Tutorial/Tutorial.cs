// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Tutorial {
    public abstract class Tutorial : MonoBehaviour {

        // State variables
        System.Guid currentGuid;

        bool _isStarted;
        public bool isStarted {
            get => _isStarted;
            private set => _isStarted = value;
        }

        protected void ShowMessage(
            string message,
            MessageType type,
            Sprite customSprite = null,
            System.Action onFullShowCallback = null,
            int? seconds = null
        ) {
            currentGuid = MessagesController.instance.AddMessage(
                message: message,
                type: type,
                seconds: seconds ?? 0,
                isFinite: seconds != null,
                customSprite: customSprite,
                hideInNewMessage: true,
                onFullShowCallback: onFullShowCallback
            );
        }

        protected void HideMessageOfCurrentState() {
            MessagesController.instance.HideMessageWithGuid(guid: currentGuid);
        }

        public void StartTutorial() {
            isStarted = true;
            CustomStartTutorial();
        }

        abstract protected void CustomStartTutorial();

        abstract public bool ReceiveSignal(string signal);

    }
}