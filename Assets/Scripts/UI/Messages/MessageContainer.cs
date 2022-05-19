// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Audio;

namespace ScrapCoder.UI {
    public class MessageContainer : MonoBehaviour, INodeExpander {

        // Editor variables
        // [SerializeField] ExpandableText titleText;
        [SerializeField] ExpandableText messageText;
        [SerializeField] NodeSprite icon;
        [SerializeField] NodeSprite customSprite;

        [SerializeField] ButtonController discardButton;

        [SerializeField] NodeTransform normalMessageParent;
        [SerializeField] NodeTransform spriteMessageParent;

        [SerializeField] NodeTransform spriteShape;
        [SerializeField] NodeTransform polygonCollider;

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        SoundScript _sound;
        SoundScript sound => _sound ??= (GetComponent<SoundScript>() as SoundScript);

        // Methods
        public void SetDiscardCallback(System.Action discardCallback) {
            discardButton.AddListener(listener: discardCallback);
        }

        public void ShowNewMessage(MessageInfo message) {
            sound.PlayClip();

            ChangeIcon(message);
            ExpandByText(message.message);

            ownTransform.SetPosition(
                y: (ownTransform.height + 50) * InterfaceCanvas.NodeScaleFactor,
                smooth: true
            );
        }

        public void Hide() {
            ownTransform.SetPosition(
                y: -20 * InterfaceCanvas.NodeScaleFactor,
                smooth: true,
                endingCallback: () => {
                    ExpandByText("");
                    MessagesController.instance.ClearCurrentMessage();
                }
            );
        }

        void ExpandByText(string newText) {
            var (dx, dy) = messageText.ChangeTextExpandingAll(newText: newText);
            ownTransform.Expand(dx: dx, dy: dy);
            ownTransform.SetPosition(x: (int)System.Math.Round(ownTransform.width * InterfaceCanvas.NodeScaleFactor / -2f));

            discardButton.ownTransform.SetPositionByDelta(dx: dx);
        }

        void ChangeIcon(MessageInfo message) {
            if (message.customIcon != null) {
                icon.SetCustomSprite(message.customIcon);
            } else {
                icon.SetState(
                    state: message.type == MessageType.Normal
                        ? "normal"
                        : message.type == MessageType.Warning
                        ? "warning"
                        : "error"
                );
            }

            icon.ownTransform.SetPosition(y: icon.ownTransform.height - 4);
        }

        public (int? dx, int? dy) Expand(int? dx = null, int? dy = null, bool smooth = false, INodeExpanded expanded = null) {
            var newWidth = ownTransform.width + (dx ?? 0);
            var newHeight = ownTransform.height + (dy ?? 0);

            spriteShape.Expand(dx: dx, dy: dy);
            polygonCollider.Expand(dx: dx, dy: dy);
            icon.ownTransform.SetPosition(x: (int)System.Math.Round(newWidth / 2f - icon.ownTransform.width / 2f));

            return (dx, dy);
        }
    }
}