// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class ButtonVisualState : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] List<NodeTransform> itemsToRight;
        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] NodeSprite spriteState;

        [SerializeField] NodeShape shapeState;
        [SerializeField] List<NodeSprite> shapeSpritesState;

        // Lazy and other variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, NodeArray _) {

            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsToRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy, smooth: smooth));

            return (dx, dy);
        }

        public void SetVisible(bool visible, string type) {
            if (type == "shape") {
                shapeState.SetVisible(visible);
                shapeSpritesState.ForEach(sprite => sprite.SetVisible(visible));

                spriteState.SetVisible(false);
            } else if (type == "sprite") {
                shapeState.SetVisible(false);
                shapeSpritesState.ForEach(sprite => sprite.SetVisible(false));

                spriteState.SetVisible(visible);
            }
        }
    }

}