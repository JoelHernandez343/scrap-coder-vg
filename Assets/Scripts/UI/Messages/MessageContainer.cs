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

        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToRight;
        [SerializeField] List<NodeTransform> itemsToCenterHorizontally;

        [SerializeField] int internalXPadding = 16;

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        SoundScript _sound;
        SoundScript sound => _sound ??= (GetComponent<SoundScript>() as SoundScript);

        int iconOffset => icon.ownTransform.height - 4;

        // Methods
        void Start() {
            discardButton.AddListener(() => MessagesController.instance.HideCurrentMessage());
        }

        public void ShowNewMessage(MessageInfo message) {
            sound.PlayClip();

            ChangeIcon(message);
            ExpandByText(message.message);

            ownTransform.SetPosition(
                y: (ownTransform.height + message.customHeight) * InterfaceCanvas.NodeScaleFactor,
                smooth: true,
                endingCallback: message.onFullShowCallback
            );
        }

        public void Hide() {
            ownTransform.SetPosition(
                y: -(20 + iconOffset) * InterfaceCanvas.NodeScaleFactor,
                smooth: true,
                endingCallback: () => Reset()
            );
        }

        void Reset() {
            ChangeIcon(message: new MessageInfo { status = MessageStatus.Normal });
            ExpandByText("");
            MessagesController.instance.ClearCurrentMessage();

            ownTransform.SetPosition(y: -20 * InterfaceCanvas.NodeScaleFactor);
        }

        void ExpandByText(string newText) {
            var (textDx, textDy) = messageText.ChangeTextExpandingAll(newText: newText);

            var newWidth = System.Math.Max(ownTransform.initWidth, messageText.currentTextWidth + 2 * internalXPadding);
            var dx = newWidth - ownTransform.width;

            ownTransform.Expand(dx: dx, dy: textDy);
            ownTransform.SetPosition(x: (int)System.Math.Round(ownTransform.width * InterfaceCanvas.NodeScaleFactor / -2f));
        }

        void ChangeIcon(MessageInfo message) {
            if (message?.customIcon != null) {
                icon.SetCustomSprite(message.customIcon);
            } else {
                icon.SetState(
                    state: message.status == MessageStatus.Normal
                        ? "normal"
                        : message.status == MessageStatus.Warning
                        ? "warning"
                        : "error"
                );
            }

            icon.ownTransform.SetPosition(y: iconOffset);
        }

        public (int? dx, int? dy) Expand(int? dx = null, int? dy = null, bool smooth = false, INodeExpanded expanded = null) {
            var newWidth = ownTransform.width + (dx ?? 0);
            var newHeight = ownTransform.height + (dy ?? 0);

            spriteShape.Expand(dx: dx, dy: dy);
            polygonCollider.Expand(dx: dx, dy: dy);

            itemsToCenterHorizontally.ForEach(i => i.SetPosition(x: (int)System.Math.Round(newWidth / 2f - i.width / 2f)));

            itemsBelow.ForEach(i => i.SetPositionByDelta(dy: -dy));
            itemsToRight.ForEach(i => i.SetPositionByDelta(dx: dx));

            return (dx, dy);
        }
    }
}