using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRepeatBehaviour : MonoBehaviour, INodeSelectorModifier, INodePartsRefresher, IZoneParentRefresher {

    [SerializeField] NodeController controller;

    [SerializeField] NodeTransform edgePiece;

    [SerializeField] NodeZone childrenZone;

    [SerializeField] NodeArray children;
    [SerializeField] NodeArray siblings;

    const int internalGap = 10;

    void INodeSelectorModifier.ModifySelectorFunc() {
        controller.selector[NodeZoneColor.Red, NodeZoneColor.Blue] = AddNodesToChildren;
        controller.selector[NodeZoneColor.Yellow, NodeZoneColor.Green] = AddNodesToChildren;
    }

    void AddNodesToChildren(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
        if (ownZone == childrenZone || ownZone.controller.parentArray == children) {
            children.AddNodes(inZone.controller, toThisNode ?? controller);
        } else {
            siblings.AddNodes(inZone.controller, toThisNode ?? controller);
        }
    }

    void INodePartsRefresher.RefreshParts(NodeArray toThisArray, (int dx, int dy)? delta) {
        var d = delta ?? (0, 0);

        if (toThisArray == children) {
            Debug.Log($"[{controller.gameObject.name}] {toThisArray.previousCount}");

            if (toThisArray.previousCount == 0) {
                d.dy -= internalGap;
            } else if (toThisArray.previousCount > 0 && toThisArray.Count == 0) {
                d.dy += internalGap;
            }

            edgePiece.Expand(dy: d.dy);
            siblings.ownTransform.SetPositionByDelta(dy: -d.dy);

            controller.ownTransform.Expand(dy: d.dy);
        }

        controller.parentArray?.RefreshParts(controller, d);
    }

    void IZoneParentRefresher.SetZonesAsParent(NodeArray array) {
        if (array == children) {
            if (array.Count == 0) {
                childrenZone.color = NodeZoneColor.Red;
            } else {
                childrenZone.color = NodeZoneColor.Yellow;
            }
        } else {
            throw new System.ArgumentException($"This array {array.gameObject.name} is unknown");
        }
    }
}
