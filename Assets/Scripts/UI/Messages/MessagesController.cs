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
        [SerializeField] MessageContainer messageContainer;

        // State variables
        MessageInfo currentMessage;
        Queue<MessageInfo> messageQueue = new Queue<MessageInfo>();

        [SerializeField] States state = States.Iddle;

        bool isFiniteDuration => waitTime >= 0f;

        float timer = 0f;
        float waitTime = 3f;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
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
        /// <param name="type">The type of the message. Accepts Normal (default), Warning and Error.</param>
        /// <param name="seconds">Duration on seconds to display the <paramref name="message"/>. Negative numbers means infinite duration.</param>
        /// <param name="isFinite">
        /// Default value is <see langword="true"/>. If <see langword="false"/>, the message will have infinite duration.
        /// Overrides <paramref name="seconds"/>.
        /// </param>
        /// <param name="customSprite">Sprite to show instead of icon</param>
        /// <param name="hideInNewMessage">Indicates whether the message is automatically hidden if a new message is added.</param>
        /// <returns>A Guid of the added message</returns>
        public System.Guid AddMessage(
            string message,
            MessageType type = MessageType.Normal,
            int seconds = 4,
            bool isFinite = true,
            Sprite customSprite = null,
            bool hideInNewMessage = false
        ) {
            var guid = System.Guid.NewGuid();

            messageQueue.Enqueue(new MessageInfo {
                message = message,
                type = type,
                customIcon = customSprite,
                guid = guid,
                hideInNewMessage = hideInNewMessage
            });

            waitTime = isFinite ? seconds : -1;

            if (currentMessage?.hideInNewMessage == true) {
                HideCurrentMessage();
            }

            return guid;
        }

        void ShowMessage() {
            timer = 0;
            currentMessage = messageQueue.Dequeue();
            messageContainer.ShowNewMessage(currentMessage);
            state = States.ShowingMessage;
        }

        public void HideCurrentMessage() {
            messageContainer.Hide();
            state = States.WaitingForHiddenMessage;
        }

        public bool HideMessage(System.Guid guid) {
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