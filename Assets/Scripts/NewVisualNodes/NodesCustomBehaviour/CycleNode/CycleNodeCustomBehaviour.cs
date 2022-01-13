using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleNodeCustomBehaviour : MonoBehaviour, INodePositioner, IZoneParentRefresher {

    [SerializeField] NodeController controller;

    [SerializeField] NodeZone childrenZone;

    [SerializeField] NodeArray siblings;
    [SerializeField] NodeArray children;

    [SerializeField] NodeTransform angleShape;

    void Start() {
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

    void INodePositioner.SetPartsPosition(NodeArray toThisArray) {

        if (toThisArray == children) {
            var dy = (children.height + 8) - angleShape.height;

            if (children.Count == 0) {
                dy += 8;
            }

            angleShape.Expand(dy: dy);
            siblings.nodeTransform.SetPositionByDelta(dy: -dy);

            controller.nodeTransform.Expand(dy: dy);
        }


        controller.parentArray?.SetPartsPosition(controller);
    }

    void IZoneParentRefresher.SetZonesAsParent(NodeArray array) {
        if (array == children) {
            if (array.Count == 0) {
                childrenZone.color = NodeZoneColor.Red;
            } else {
                childrenZone.color = NodeZoneColor.Yellow;
            }
        } else {
            throw new System.ArgumentException($"This array {array.gameObject.name} is unkown");
        }
    }
}
