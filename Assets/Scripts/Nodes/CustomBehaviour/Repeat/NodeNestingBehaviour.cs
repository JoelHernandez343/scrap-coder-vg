using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeNestingBehaviour : MonoBehaviour, INodeSelectorModifier {

    [System.Serializable]
    struct TupleNodeChildrenZone {
        public NodeArray children;
        public NodeZone zone;
    }

    [SerializeField] NodeController controller;
    [SerializeField] NodeArray siblings;

    [SerializeField] List<TupleNodeChildrenZone> listOfChildren;

    void INodeSelectorModifier.ModifySelectorFunc() {
        controller.selector[NodeZoneColor.Red, NodeZoneColor.Blue] = AddNodesToChildren;
        controller.selector[NodeZoneColor.Yellow, NodeZoneColor.Green] = AddNodesToChildren;
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
}
