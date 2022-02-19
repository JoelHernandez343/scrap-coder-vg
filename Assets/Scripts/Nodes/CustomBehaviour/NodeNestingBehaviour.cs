// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeNestingBehaviour : MonoBehaviour, INodeSelectorModifier, INodePartsAdjuster, IZoneParentRefresher {

        [SerializeField] NodeController controller;
        [SerializeField] NodeArray siblings;

        [SerializeField] List<NodeContainer> containers;
        [SerializeField] List<NodeTransform> componentParts;

        void INodeSelectorModifier.ModifySelectorFunc() {
            controller.selector[ZoneColor.Red, ZoneColor.Blue] = AddNodesToChildren;
            controller.selector[ZoneColor.Yellow, ZoneColor.Green] = AddNodesToChildren;
        }

        void AddNodesToChildren(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            var wereAdded = false;

            foreach (var container in containers) {
                var children = container.array;
                var zone = container.zone;

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

        (int dx, int dy) INodePartsAdjuster.AdjustParts(NodeArray fromThisArray, (int dx, int dy)? delta) {
            var newDelta = delta ?? (0, 0);

            var container = containers.Find(container => container.array == fromThisArray);
            var children = container.array;
            var pieceToExpand = container.pieceToExpand;

            if (children.previousCount == 0) {
                newDelta.dy -= container.defaultHeight;
                newDelta.dx -= container.defaultWidth;
            } else if (children.Count == 0) {
                newDelta.dy += container.defaultHeight;
                newDelta.dx += container.defaultWidth;
            }

            newDelta.dx = container.modifyWidthOfPiece ? newDelta.dx : 0;
            newDelta.dy = container.modifyHeightOfPiece ? newDelta.dy : 0;

            pieceToExpand.ownTransform.Expand(dx: newDelta.dx, dy: newDelta.dy, fromThisArray);

            AdjustVerticalParts(pieceToExpand.ownTransform, newDelta);

            return newDelta;
        }

        void AdjustVerticalParts(NodeTransform pieceModified, (int dx, int dy) delta) {
            var begin = componentParts.IndexOf(pieceModified) + 1;

            for (var i = begin; i < componentParts.Count; ++i) {
                componentParts[i].SetPositionByDelta(dy: -delta.dy);
                componentParts[i].Expand(dx: delta.dx);
            }
        }

        void IZoneParentRefresher.SetZonesAsParent(NodeArray array) {
            var container = containers.Find(container => container.array == array);

            container.zone.color = container.array.Count == 0
                ? ZoneColor.Red
                : ZoneColor.Yellow;
        }
    }

}