// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ExpandableText : MonoBehaviour, INodeExpander {

        // State variables
        [SerializeField] TextMeshPro textMeshPro;

        // Lazy state variables
        public string text {
            get => textMeshPro.text;
            private set => textMeshPro.text = value;
        }

        // Lazy and other variables
        int previousTextWidth;

        int currentTextWidth => characterCount > 0
            ? (int)System.Math.Round(lastCharacterInfo.topRight.x)
            : 0;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public int characterCount => textMeshPro.textInfo.characterCount;

        public TMP_CharacterInfo[] characterInfo => textMeshPro.textInfo.characterInfo;

        public TMP_CharacterInfo lastCharacterInfo => characterInfo[characterCount - 1];

        // Methods
        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, INodeExpandable _) {
            var size = ownTransform.rectTransform.sizeDelta;

            size.x += dx;
            size.y += dy;

            ownTransform.rectTransform.sizeDelta = size;

            return (dx, dy);
        }

        public int ChangeText(string newText, int minWidth, int lettersOffset) {
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