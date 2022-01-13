/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfController : NodeController3 {

    NodePart topPart, bodyPart, bottomPart;
    NodeContainer nodeContainer;
    NodeTrigger triggerContainer;

    [SerializeField] float leftAnchor;

    public override void Init() {

        topPart = transform.Find("TopPart").GetComponent<NodePart>();
        bodyPart = transform.Find("BodyPart").GetComponent<NodePart>();
        bottomPart = transform.Find("BottomPart").GetComponent<NodePart>();

        nodeContainer = new NodeContainer(
            controller: this,
            asidePart: bodyPart,
            bottomPart: bottomPart
        );

        triggerContainer = FindTrigger("ContainerTrigger");

        leftAnchor = bodyPart.width;

        nodeContainer.SetPosition((0, -topPart.height));

        SetDimensions();

        siblings.SetPosition((0, -height), init: true);
    }

    NodeTrigger FindTrigger(string name) {
        return ownTriggers.Find(trigger => trigger.gameObject.name == name);
    }

    protected override bool SetTriggersAsParent(NodeList list) {
        if (base.SetTriggersAsParent(list)) {
            return true;
        }

        if (list == nodeContainer.nodes) {
            if (list.Count == 0) {
                triggerContainer.color = TriggerColor.Red;
            } else {
                triggerContainer.color = TriggerColor.Yellow;
            }
            return true;
        }

        return false;
    }

    protected override void OnTriggerRedThenBlue(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        if (ownTrigger == triggerContainer || ownTrigger.controller.parentList == nodeContainer.nodes) {
            nodeContainer.nodes.AddNodes(inTrigger.controller, toThisNode ?? this);
        } else {
            siblings.AddNodes(inTrigger.controller, toThisNode ?? this);
        }
    }

    protected override void OnTriggerYellowThenGreen(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        if (ownTrigger == triggerContainer || ownTrigger.controller.parentList == nodeContainer.nodes) {
            nodeContainer.nodes.AddNodes(inTrigger.controller, toThisNode ?? this);
        } else {
            siblings.AddNodes(inTrigger.controller, toThisNode ?? this);
        }
    }

    public override void SetPartsPosition(NodeController3 node = null) {
        if (node?.parentList == siblings) return;

        nodeContainer.SetPosition((0, -topPart.height));

        SetDimensions();

        siblings.SetPosition((0, -height));

        parentList?.SetPartsPosition(this);
    }

    public override void SetDimensions() {
        height = topPart.height + nodeContainer.height;
        width =
            topPart.width < (nodeContainer.width + bodyPart.width)
            ? nodeContainer.width + bodyPart.width
            : topPart.width;
    }
}
