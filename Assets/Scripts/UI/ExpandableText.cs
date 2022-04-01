using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class ExpandableText : MonoBehaviour, VisualNodes.INodeExpander {

        // Editor variables
        [SerializeField] VisualNodes.NodeTransform ownTransform;

        // Methods
        (int dx, int dy) VisualNodes.INodeExpander.Expand(int dx, int dy, bool smooth, VisualNodes.NodeArray _) {
            var size = ownTransform.rectTransform.sizeDelta;

            size.x += dx;
            size.y += dy;

            ownTransform.rectTransform.sizeDelta = size;

            return (dx, dy);
        }
    }
}