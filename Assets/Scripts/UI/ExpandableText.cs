// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ExpandableText : MonoBehaviour, INodeExpander {

        // Lazy state variables
        public string text {
            get => textMeshPro.text;
            private set => textMeshPro.text = value;
        }

        // Lazy and other variables
        int previousTextWidth;

        TextMeshPro _textMeshPro;
        TextMeshPro textMeshPro => _textMeshPro ??= (GetComponent<TextMeshPro>() as TextMeshPro);

        public int currentTextWidth {
            get {
                if (characterCount == 0) return 0;

                var maxRight = 0;
                for (var i = 0; i < characterCount; ++i) {
                    if (maxRight < characterInfo[i].topRight.x) {
                        maxRight = (int)System.Math.Round(characterInfo[i].topRight.x);
                    }
                }

                return maxRight;
            }
        }

        public int currentTextHeight {
            get {
                if (characterCount == 0) return 13;

                var maxBottom = 0;
                for (var i = 0; i < characterCount; ++i) {
                    if (maxBottom > characterInfo[i].bottomRight.y) {
                        maxBottom = (int)System.Math.Round(characterInfo[i].bottomRight.y);
                    }
                }

                return -maxBottom;
            }
        }

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public int characterCount => textMeshPro.textInfo?.characterCount ?? 0;

        public TMP_CharacterInfo[] characterInfo => textMeshPro.textInfo.characterInfo;

        TMP_CharacterInfo lastCharacterInfo => characterInfo[characterCount - 1];

        // Methods
        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded _) {
            var size = ownTransform.rectTransform.sizeDelta;

            size.x += dx ?? 0;
            size.y += dy ?? 0;

            ownTransform.rectTransform.sizeDelta = size;

            return (dx, dy);
        }

        public int ChangeText(string newText, int minWidth = 0, int lettersOffset = 0) {
            if (text == newText) return 0;

            // Update text
            text = newText;

            // Store old width
            previousTextWidth = currentTextWidth;

            // Refresh text dimensions
            textMeshPro.ForceMeshUpdate(ignoreActiveState: true);

            var textDelta = ExpandTextBox();
            var delta = CalculateDelta(textDelta, minWidth, lettersOffset);

            // Update previous width
            previousTextWidth = currentTextWidth;

            // Update parents with delta
            return delta;
        }

        public (int dx, int dy) ChangeTextExpandingAll(string newText) {
            if (text == newText) return (dx: 0, dy: 0);

            text = newText;

            var previousTextWidth = currentTextWidth;
            var previousTextHeight = currentTextHeight;

            textMeshPro.ForceMeshUpdate(ignoreActiveState: true);

            var dx = currentTextWidth - previousTextWidth;
            var dy = currentTextHeight - previousTextHeight;

            return (dx: dx, dy: dy);
        }

        int ExpandTextBox() {
            var textDelta = currentTextWidth - previousTextWidth;
            ownTransform.Expand(dx: textDelta);

            return textDelta;
        }

        int CalculateDelta(int textDelta, int minWidth, int lettersOffset) {
            var delta = 0;
            var initWidth = minWidth - lettersOffset < 0 ? 0 : minWidth - lettersOffset;

            var previous = previousTextWidth;
            var current = currentTextWidth;

            if (previous < initWidth && current <= initWidth) {
                delta = 0;
            } else if (previous <= initWidth && current > initWidth) {
                delta = current - initWidth;
            } else if (previous >= initWidth && current > initWidth) {
                delta = textDelta;
            } else if (previous >= initWidth && current <= initWidth) {
                delta = initWidth - previous;
            }

            return delta;
        }
    }
}