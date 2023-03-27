// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class MessagesController : MonoBehaviour {

        // Internal types
        enum States { Iddle, ShowingMessage, WaitingForHiddenMessage }

        // Static variables
        public static MessagesController instance;

        // Editor variable
        [SerializeField] MessageContainer informationMessageContainer;
        [SerializeField] MessageContainer acceptAndCancelMessageContainer;

        // State variables
        MessageInfo currentMessage;
        MessageContainer currentMessageContainer;
        Queue<MessageInfo> messageQueue = new Queue<MessageInfo>();

        [SerializeField] States state = States.Iddle;

        bool isFiniteDuration => waitTime >= 0f;

        float timer = 0f;
        float waitTime = 3f;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }

            instance = this;
        }

        void Update() {

            if (state == States.ShowingMessage && isFiniteDuration) {
                AddToTimer();
            } else if (state == States.Iddle && messageQueue.Count > 0) {
                ShowMessage();
            }

        }

        void AddToTimer() {
            timer += Time.deltaTime;

            if (timer >= waitTime) {
                HideCurrentMessage();
                timer = 0;
            }
        }

        /// <summary>
        /// Function to queue messages and display to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="status">The status of the message. Accepts Normal (default), Warning and Error.</param>
        /// <param name="seconds">Duration on seconds to display the <paramref name="message"/>. Negative numbers means infinite duration.</param>
        /// <param name="isFinite">
        /// Default value is <see langword="true"/>. If <see langword="false"/>, the message will have infinite duration.
        /// Overrides <paramref name="seconds"/>.
        /// </param>
        /// <param name="customSprite">Sprite to show instead of icon</param>
        /// <param name="hideInNewMessage">Indicates whether the message is automatically hidden if a new message is added.</param>
        /// <param name="onFullShowCallback">Callback to execute when the message is fully showed.</param>
        /// <param name="customHeight">Height where show the message.</param>
        /// <param name="type">The type of the message. It help to decide between different containers</param>
        /// <returns>A Guid of the added message</returns>
        public System.Guid AddMessage(
            string message,
            MessageStatus status = MessageStatus.Normal,
            int seconds = 4,
            bool isFinite = true,
            Sprite customSprite = null,
            bool hideInNewMessage = false,
            System.Action onFullShowCallback = null,
            int customHeight = 50,
            MessageType type = MessageType.Information
        ) {
            var guid = System.Guid.NewGuid();

            messageQueue.Enqueue(new MessageInfo {
                message = message,
                status = status,
                customIcon = customSprite,
                guid = guid,
                hideInNewMessage = hideInNewMessage,
                onFullShowCallback = onFullShowCallback,
                seconds = isFinite ? seconds : -1,
                customHeight = customHeight,
                type = type,
            });

            if (currentMessage?.hideInNewMessage == true) {
                HideCurrentMessage();
            }

            return guid;
        }

        void ShowMessage() {
            currentMessage = messageQueue.Dequeue();

            currentMessageContainer = currentMessage.type == MessageType.Information
                ? informationMessageContainer
                : acceptAndCancelMessageContainer;

            currentMessageContainer.ShowNewMessage(currentMessage);
            state = States.ShowingMessage;
            waitTime = currentMessage.seconds;

            timer = 0;
        }

        public void HideCurrentMessage() {
            currentMessageContainer.Hide();
            state = States.WaitingForHiddenMessage;
        }

        public bool HideMessageWithGuid(System.Guid guid) {
            if (currentMessage?.guid == guid) {
                HideCurrentMessage();
                return true;
            }

            return false;
        }

        public void ClearCurrentMessage() {
            currentMessage = null;
            state = States.Iddle;
        }

    }
}