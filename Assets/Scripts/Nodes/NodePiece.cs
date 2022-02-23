// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodePiece : MonoBehaviour, INodeExpander {

        [SerializeField] public NodeTransform ownTransform;

        [SerializeField] List<NodeTransform> horizontalItems;
        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToExpand;

        public NodeController controller => ownTransform.controller;

        int maxHeight = 0;

        void Start() {
            maxHeight = getMaxHeight();
        }

        public bool HasHorizontalArray(NodeArray array)
            => GetIndexOfHorizontalArray(array) != -1;

        int GetIndexOfHorizontalArray(NodeArray array)
            => horizontalItems.FindIndex(item => item.GetComponent<NodeContainer>()?.array == array);

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray toThisArray) {

            var modified = GetIndexOfHorizontalArray(toThisArray);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalItems.Count; ++i) {
                    horizontalItems[i].SetPositionByDelta(dx: dx);
                }

                dy = centerHorizontalItems(horizontalItems[modified], dy);
            }


            // horizontalItems.ForEach(item => {
            //     var container = item.GetComponent<NodeContainer>();

            //     if (container == null) {
            //         item.SetFloatPositionByDelta(dy: -dy / 2f);
            //     } else {
            //         container.sprite.ownTransform.SetFloatPositionByDelta(dy: -dy / 2f);
            //     }
            // });
            itemsToExpand.ForEach(item => item.Expand(dx, dy));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy));

            return (dx, dy);
        }

        int centerHorizontalItems(NodeTransform modified, int dy) {
            var (delta, center) = calHorizontalDelta(modified, dy);

            if (center != "nothing") {
                horizontalItems.ForEach(item => {
                    if (center == "all" || item != modified) {
                        item.SetFloatPositionByDelta(dy: -delta / 2f);
                    }
                });
            }

            return delta;
        }

        int getMaxHeight() {
            var maxHeight = 0;
            foreach (var item in horizontalItems) {
                maxHeight = item.height > maxHeight ? item.height : maxHeight;
            }
            return maxHeight;
        }

        (int delta, string center) calHorizontalDelta(NodeTransform modified, int dy) {
            var currentMaxHeight = getMaxHeight();
            var delta = currentMaxHeight - maxHeight;

            if (delta == 0) {
                modified.SetFloatPositionByDelta(dy: dy / 2f);
                return (0, "nothing");
            }

            maxHeight = currentMaxHeight;

            if (delta < 0) {
                modified.SetFloatPositionByDelta(dy: dy / 2f);
                return (delta, "all");
            }

            modified.ResetYToRelative();
            var position = new Vector2(modified.x, modified.y);
            var diff = modified.relativeOrigin - position;
            delta += (int)diff.y;

            return (delta, "all_wo_max");
        }
    }
}