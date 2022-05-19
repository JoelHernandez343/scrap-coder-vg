// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class MessagesController : MonoBehaviour {

        // Static variables
        public static MessagesController instance;

        // Editor variable
        [SerializeField] MessageContainer messageContainer;

        // State variables
        MessageInfo currentMessage;
        Queue<MessageInfo> messageQueue = new Queue<MessageInfo>();

        // Lazy and other variables
        float timer = 0f;
        float waitTime = 10f;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Start() {
            messageContainer.SetDiscardCallback(() => HideMessage());
        }

        void Update() {

            if (currentMessage != null) {
                AddToTimer();
            } else if (messageQueue.Count > 0) {
                ShowMessage();
            }

        }

        void AddToTimer() {
            timer += Time.deltaTime;

            if (timer >= waitTime) {
                HideMessage();
                timer = 0;
            }
        }

        public void AddMessage(string message, MessageType type = MessageType.Normal, Sprite customSprite = null) {
            messageQueue.Enqueue(new MessageInfo {
                message = message,
                type = type,
                customSprite = customSprite
            });
        }

        void ShowMessage() {
            timer = 0;
            currentMessage = messageQueue.Dequeue();
            messageContainer.ShowNewMessage(currentMessage);
        }

        void HideMessage() {
            messageContainer.Hide();
            currentMessage = null;
        }

    }
}