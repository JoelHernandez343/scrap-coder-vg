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

        [SerializeField] List<TupleNodeTransformArray> horizontalElements;

        [SerializeField] new NodeTransform collider;
        [SerializeField] NodeTransform unionSprite;
        [SerializeField] NodeTransform middleZone;
        [SerializeField] NodeTransform bottomZone;
        [SerializeField] NodeTransform children;
        [SerializeField] NodeTransform shape;

        [SerializeField] public NodeTransform ownTransform;

        public NodeController controller => ownTransform.controller;

        public bool HasHorizontalArray(NodeArray array) => GetIndexOfHorizontalArray(array) != -1;

        int GetIndexOfHorizontalArray(NodeArray array) => horizontalElements.FindIndex(e => e.nodeArray == array);

        void INodeExpander.Expand(int dx, int dy, NodeArray toThisArray) {

            var modified = GetIndexOfHorizontalArray(toThisArray);
            if (modified != -1) {
                for (var i = modified + 1; i < horizontalElements.Count; ++i) {
                    horizontalElements[i].transform.SetPositionByDelta(dx: dx);
                }

                horizontalElements.ForEach(e => e.transform.SetFloatPositionByDelta(dy: -dy / 2f));
            }

            collider?.Expand(dx, dy);
            shape?.Expand(dx, dy);

            unionSprite?.SetPositionByDelta(dy: -dy);
            bottomZone?.SetPositionByDelta(dy: -dy);
            children?.SetPositionByDelta(dy: -dy);
        }
    }
}