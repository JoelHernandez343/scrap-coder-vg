using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConditionalElseBehaviour : MonoBehaviour, INodePositioner, IZoneParentRefresher {

    [SerializeField] NodeController controller;

    [SerializeField] NodeZone childrenZone1;
    [SerializeField] NodeZone childrenZone2;

    [SerializeField] NodeArray siblings;

    [SerializeField] NodeArray children1;
    [SerializeField] NodeArray children2;

    [SerializeField] NodeTransform middlePiece;
    [SerializeField] NodeTransform bottomPiece;

    [SerializeField] NodeTransform middleText;

    void Start() {
        controller.selector[NodeZoneColor.Red, NodeZoneColor.Blue] = AddNodesToChildren;
        controller.selector[NodeZoneColor.Yellow, NodeZoneColor.Green] = AddNodesToChildren;
    }

    void AddNodesToChildren(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
        if (ownZone == childrenZone1 || ownZone.controller.parentArray == children1) {
            children1.AddNodes(inZone.controller, toThisNode ?? controller);
        } else if (ownZone == childrenZone2 || ownZone.controller.parentArray == children2) {
            children2.AddNodes(inZone.controller, toThisNode ?? controller);
        } else {
            siblings.AddNodes(inZone.controller, toThisNode ?? controller);
        }
    }

    void INodePositioner.SetPartsPosition(NodeArray toThisArray) {
        if (toThisArray == children1) {
            SetPartsPosition(children1, middlePiece, bottomPiece);
        } else if (toThisArray == children2) {
            SetPartsPosition(children2, bottomPiece);
        }

        controller.parentArray?.SetPartsPosition(controller);
    }

    void SetPartsPosition(NodeArray children, NodeTransform piece, NodeTransform otherPiece = null) {
        var offset = piece == middlePiece ? 20 : 8;
        var dy = (children.height + offset) - piece.height;

        if (children.Count == 0) {
            dy += 8;
        }

        piece.Expand(dy: dy);

        otherPiece?.SetFloatPositionByDelta(dy: -dy);
        siblings.nodeTransform.SetPositionByDelta(dy: -dy);

        if (children == children1) {
            middleText.SetPositionByDelta(dy: -dy);
            children2.nodeTransform.SetPositionByDelta(dy: -dy);
        }

        controller.nodeTransform.Expand(dy: dy);
    }

    void IZoneParentRefresher.SetZonesAsParent(NodeArray array) {
        if (array == children1) {
            SetZonesAsParent(children1, childrenZone1);
        } else if (array == children2) {
            SetZonesAsParent(children2, childrenZone2);
        } else {
            throw new System.ArgumentException($"This array {array.gameObject.name} is unkown");
        }
    }

    void SetZonesAsParent(NodeArray children, NodeZone zone) {
        if (children.Count == 0) {
            zone.color = NodeZoneColor.Red;
        } else {
            zone.color = NodeZoneColor.Yellow;
        }
    }
}
