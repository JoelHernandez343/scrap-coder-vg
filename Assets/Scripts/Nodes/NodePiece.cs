// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodePiece : MonoBehaviour, INodeExpander {

        [System.Serializable]
        struct TupleNodeTransformArray {
            public NodeTransform transform;
            public NodeArray nodeArray;
        }

        [SerializeField] List<TupleNodeTransformArray> horizontalItems;
        [SerializeField] List<NodeTransform> itemsBelow;
        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] public NodeTransform ownTransform;

        public NodeController controller => ownTransform.controller;

        public bool HasHorizontalArray(NodeArray array) => GetIndexOfHorizontalArray(array) != -1;

        int GetIndexOfHorizontalArray(NodeArray array) => horizontalItems.FindIndex(e => e.nodeArray == array);

        void INodeExpander.Expand(int dx, int dy, NodeArray toThisArray) {

            var modified = GetIndexOfHorizontalArray(toThisArray);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalItems.Count; ++i) {
                    horizontalItems[i].transform.SetPositionByDelta(dx: dx);
                }
            }

            horizontalItems.ForEach(item => item.transform.SetFloatPositionByDelta(dy: -dy / 2f));
            itemsToExpand.ForEach(item => item.Expand(dx, dy));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy));
        }
    }
}