// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodePiece : MonoBehaviour, INodeExpander {


        [SerializeField] List<NodeTransform> horizontalItems;
        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] public NodeTransform ownTransform;

        public NodeController controller => ownTransform.controller;

        public bool HasHorizontalArray(NodeArray array)
            => GetIndexOfHorizontalArray(array) != -1;

        int GetIndexOfHorizontalArray(NodeArray array)
            => horizontalItems.FindIndex(item => item.GetComponent<NodeArray>() == array);

        void INodeExpander.Expand(int dx, int dy, NodeArray toThisArray) {

            var modified = GetIndexOfHorizontalArray(toThisArray);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalItems.Count; ++i) {
                    horizontalItems[i].SetPositionByDelta(dx: dx);
                }
            }

            horizontalItems.ForEach(item => item.SetFloatPositionByDelta(dy: -dy / 2f));
            itemsToExpand.ForEach(item => item.Expand(dx, dy));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy));
        }
    }
}