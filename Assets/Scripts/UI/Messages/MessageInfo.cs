// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class MessageInfo {

        public string message;
        public MessageStatus status;
        public Sprite customIcon;
        public System.Guid guid;
        public bool hideInNewMessage;
        public int seconds;
        public System.Action onFullShowCallback;
        public int customHeight;
        public MessageType type;

    }
}