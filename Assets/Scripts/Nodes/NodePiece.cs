// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodePiece : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] List<NodeTransform> horizontalItems;
        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToTheRight;
        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] List<NodeShapeContainer> shapesOfState;
        [SerializeField] List<NodeSprite> spritesOfState;

        // State variables
        int? previousMaxHeight;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        void Start() {
            previousMaxHeight ??= getMaxHeight();
        }

        int GetIndexOfHorizontalExpandable(INodeExpanded expandable)
            => horizontalItems.FindIndex(item => item.GetComponent<INodeExpanded>() == expandable);

        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded expandable) {

            var modified = GetIndexOfHorizontalExpandable(expandable);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalItems.Count; ++i) {
                    horizontalItems[i].SetPositionByDelta(dx: dx, smooth: smooth);
                }

                dy = centerHorizontalItems(modified: horizontalItems[modified], dy: dy, smooth: smooth);
            }

            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy, smooth: smooth));
            itemsToTheRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));

            return (dx, dy);
        }

        int? centerHorizontalItems(NodeTransform modified, int? dy, bool smooth) {
            if (dy == 0 || dy == null) {
                return null;
            }

            var (delta, center) = calHorizontalDelta(modified: modified, dy: dy, smooth: smooth);

            if (center != "nothing") {
                horizontalItems.ForEach(item => {
                    if (center == "all" || item != modified) {
                        item.SetFloatPositionByDelta(dy: -delta / 2f, smooth: smooth);
                    }
                });
            }

            return delta;
        }

        int getMaxHeight() {
            var maxHeight = 0;

            horizontalItems.ForEach(item => maxHeight = item.height > maxHeight ? item.height : maxHeight);

            return maxHeight;
        }

        (int? delta, string center) calHorizontalDelta(NodeTransform modified, int? dy, bool smooth) {
            var currentMaxHeight = getMaxHeight();
            var delta = currentMaxHeight - (int)previousMaxHeight;

            // If modified is not the longest, then nothing more will be centered
            if (delta == 0) {
                modified.SetFloatPositionByDelta(dy: dy / 2f, smooth: smooth);
                return (null, "nothing");
            }

            previousMaxHeight = currentMaxHeight;

            // The longest decreased
            if (delta < 0) {
                // Adjust itself to itself, then center all to the new longest if any
                modified.SetFloatPositionByDelta(dy: dy / 2f, smooth: smooth);
                return (delta, "all");
            }

            // The longest increased, then all must be centered except itself
            // Longest returns to its original place
            modified.ResetYToRelative(smooth: smooth);

            return (delta, "all_wo_max");
        }

        public void SetState(string state) {
            shapesOfState.ForEach(s => s.SetState(state));
            spritesOfState.ForEach(s => s.SetState(state));
        }
    }
}