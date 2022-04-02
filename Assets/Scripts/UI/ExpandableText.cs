// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class ExpandableText : MonoBehaviour, VisualNodes.INodeExpander {

        // Lazy variables
        VisualNodes.NodeTransform _ownTransform;
        VisualNodes.NodeTransform ownTransform => _ownTransform ??= GetComponent<VisualNodes.NodeTransform>();

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