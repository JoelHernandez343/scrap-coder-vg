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

        // State variables
        int? previousMaxHeight;

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        void Start() {
            previousMaxHeight ??= getMaxHeight();
        }

        int GetIndexOfHorizontalExpandable(INodeExpandable expandable)
            => horizontalItems.FindIndex(item => item.GetComponent<INodeExpandable>() == expandable);

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, INodeExpandable expandable) {

            var modified = GetIndexOfHorizontalExpandable(expandable);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalItems.Count; ++i) {
                    horizontalItems[i].SetPositionByDelta(dx: dx, smooth: smooth);
                }

                dy = centerHorizontalItems(horizontalItems[modified], dy, smooth: smooth);
            }

            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy, smooth: smooth));
            itemsToTheRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));

            return (dx, dy);
        }

        int centerHorizontalItems(NodeTransform modified, int dy, bool smooth) {
            var (delta, center) = calHorizontalDelta(modified, dy, smooth);

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

        (int delta, string center) calHorizontalDelta(NodeTransform modified, int dy, bool smooth) {
            var currentMaxHeight = getMaxHeight();
            var delta = currentMaxHeight - (int)previousMaxHeight;

            // If modified is not the longest, then nothing more will be centered
            if (delta == 0) {
                modified.SetFloatPositionByDelta(dy: dy / 2f, smooth: smooth);
                return (0, "nothing");
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
    }
}