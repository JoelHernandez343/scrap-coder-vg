// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeNestingBehaviour : MonoBehaviour, INodeSelectorModifier, INodePartsRefresher, IZoneParentRefresher {

        [System.Serializable]
        struct TupleNodeChildrenZone {
            public NodeArray children;
            public NodePiece piece;
            public NodeZone zone;
        }

        [SerializeField] NodeController controller;
        [SerializeField] NodeArray siblings;

        [SerializeField] List<TupleNodeChildrenZone> listOfChildren;
        [SerializeField] List<NodeTransform> componentParts;

        const int internalGap = 10;

        void INodeSelectorModifier.ModifySelectorFunc() {
            controller.selector[ZoneColor.Red, ZoneColor.Blue] = AddNodesToChildren;
            controller.selector[ZoneColor.Yellow, ZoneColor.Green] = AddNodesToChildren;
        }

        void AddNodesToChildren(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            var wereAdded = false;

            foreach (var tuple in listOfChildren) {
                var children = tuple.children;
                var zone = tuple.zone;

                if (ownZone == zone || ownZone.controller.parentArray == children) {
                    children.AddNodes(inZone.controller, toThisNode ?? controller);
                    wereAdded = true;
                    break;
                }
            }

            if (!wereAdded) {
                siblings.AddNodes(inZone.controller, toThisNode ?? controller);
            }
        }

        (int dx, int dy) INodePartsRefresher.RefreshParts(NodeArray fromThisArray, (int dx, int dy)? delta) {
            var newDelta = delta ?? (0, 0);

            var modified = listOfChildren.FindIndex(tuple => tuple.children == fromThisArray);

            var children = listOfChildren[modified].children;
            var piece = listOfChildren[modified].piece;

            if (!piece.HasHorizontalArray(fromThisArray)) {
                if (children.previousCount == 0) {
                    newDelta.dy -= internalGap;
                } else if (children.Count == 0) {
                    newDelta.dy += internalGap;
                }
            }

            piece.ownTransform.Expand(dy: newDelta.dy, fromThisArray: fromThisArray);

            var begin = componentParts.IndexOf(piece.ownTransform) + 1;
            for (var i = begin; i < componentParts.Count; ++i) {
                componentParts[i].SetPositionByDelta(dy: -newDelta.dy);
            }

            controller.ownTransform.Expand(dy: newDelta.dy);

            return newDelta;
        }

        void IZoneParentRefresher.SetZonesAsParent(NodeArray array) {
            var tuple = listOfChildren.Find(tuple => tuple.children == array);

            tuple.zone.color = tuple.children.Count == 0
                ? ZoneColor.Red
                : ZoneColor.Yellow;
        }
    }

}