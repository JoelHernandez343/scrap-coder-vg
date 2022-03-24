// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodePiece : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] NodeTransform ownTransform;

        [SerializeField] List<NodeTransform> horizontalItems;
        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToTheRight;
        [SerializeField] List<NodeTransform> itemsToExpand;

        // State variables
        int? previousMaxHeight;

        // Methods
        void Start() {
            previousMaxHeight ??= getMaxHeight();
        }

        public bool HasHorizontalArray(NodeArray array)
            => GetIndexOfHorizontalArray(array) != -1;

        int GetIndexOfHorizontalArray(NodeArray array)
            => horizontalItems.FindIndex(item => item.GetComponent<NodeContainer>()?.array == array);

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray toThisArray) {

            var modified = GetIndexOfHorizontalArray(toThisArray);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalItems.Count; ++i) {
                    horizontalItems[i].SetPositionByDelta(dx: dx, smooth: true);
                }

                dy = centerHorizontalItems(horizontalItems[modified], dy);
            }

            itemsToExpand.ForEach(item => item.Expand(dx, dy));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy, smooth: true));
            itemsToTheRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: true));

            return (dx, dy);
        }

        int centerHorizontalItems(NodeTransform modified, int dy) {
            var (delta, center) = calHorizontalDelta(modified, dy);

            if (center != "nothing") {
                horizontalItems.ForEach(item => {
                    if (center == "all" || item != modified) {
                        item.SetFloatPositionByDelta(dy: -delta / 2f, smooth: true);
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

        (int delta, string center) calHorizontalDelta(NodeTransform modified, int dy) {
            var currentMaxHeight = getMaxHeight();
            var delta = currentMaxHeight - (int)previousMaxHeight;

            // If modified is not the longest, then nothing more will be centered
            if (delta == 0) {
                modified.SetFloatPositionByDelta(dy: dy / 2f, smooth: true);
                return (0, "nothing");
            }

            previousMaxHeight = currentMaxHeight;

            // The longest decreased
            if (delta < 0) {
                // Adjust itself to itself, then center all to the new longest if any
                modified.SetFloatPositionByDelta(dy: dy / 2f, smooth: true);
                return (delta, "all");
            }

            // The longest increased, then all must be centered except itself
            // Longest returns to its original place
            modified.ResetYToRelative(smooth: true);

            return (delta, "all_wo_max");
        }
    }
}