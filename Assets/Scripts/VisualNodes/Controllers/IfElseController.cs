/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfElseController : NodeController3 {

    NodePart topPart;

    // First container
    NodePart bodyPartFirst, middlePart;
    NodeContainer nodeContainerFirst;
    NodeTrigger triggerContainerFirst;

    // Second container
    NodePart bodyPartSecond, bottomPart;
    NodeContainer nodeContainerSecond;
    NodeTrigger triggerContainerSecond;

    float leftAnchorFirst, leftAnchorSecond;


    public override void Init() {
        topPart = transform.Find("TopPart").GetComponent<NodePart>();

        // First container
        bodyPartFirst = transform.Find("BodyPartFirst").GetComponent<NodePart>();
        middlePart = transform.Find("MiddlePart").GetComponent<NodePart>();

        nodeContainerFirst = new NodeContainer(
            controller: this,
            asidePart: bodyPartFirst,
            bottomPart: middlePart
        );
        triggerContainerFirst = FindTrigger("ContainerTriggerFirst");

        // Second container
        bodyPartSecond = transform.Find("BodyPartSecond").GetComponent<NodePart>();
        bottomPart = transform.Find("BottomPart").GetComponent<NodePart>();

        // Must change to container.height
        nodeContainerSecond = new NodeContainer(
            controller: this,
            asidePart: bodyPartSecond,
            bottomPart: bottomPart
        ); ;
        triggerContainerSecond = FindTrigger("ContainerTriggerSecond");

        leftAnchorFirst = bodyPartFirst.width;
        leftAnchorSecond = bodyPartSecond.width;

        nodeContainerFirst.SetPosition((0, -topPart.height));
        nodeContainerSecond.SetPosition((0, nodeContainerFirst.endPosition.y));

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

        if (list == nodeContainerFirst.nodes) {
            if (list.Count == 0) {
                triggerContainerFirst.color = TriggerColor.Red;
            } else {
                triggerContainerFirst.color = TriggerColor.Yellow;
            }
            return true;
        }

        if (list == nodeContainerSecond.nodes) {
            if (list.Count == 0) {
                triggerContainerSecond.color = TriggerColor.Red;
            } else {
                triggerContainerSecond.color = TriggerColor.Yellow;
            }
            return true;
        }

        return false;
    }

    protected override void OnTriggerRedThenBlue(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        if (ownTrigger == triggerContainerFirst || ownTrigger.controller.parentList == nodeContainerFirst.nodes) {
            nodeContainerFirst.nodes.AddNodes(inTrigger.controller, toThisNode ?? this);
        } else if (ownTrigger == triggerContainerSecond || ownTrigger.controller.parentList == nodeContainerSecond.nodes) {
            nodeContainerSecond.nodes.AddNodes(inTrigger.controller, toThisNode ?? this);
        } else {
            siblings.AddNodes(inTrigger.controller, toThisNode ?? this);
        }
    }

    protected override void OnTriggerYellowThenGreen(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        if (ownTrigger == triggerContainerFirst || ownTrigger.controller.parentList == nodeContainerFirst.nodes) {
            nodeContainerFirst.nodes.AddNodes(inTrigger.controller, toThisNode ?? this);
        } else if (ownTrigger == triggerContainerSecond || ownTrigger.controller.parentList == nodeContainerSecond.nodes) {
            nodeContainerSecond.nodes.AddNodes(inTrigger.controller, toThisNode ?? this);
        } else {
            siblings.AddNodes(inTrigger.controller, toThisNode ?? this);
        }
    }

    public override void SetPartsPosition(NodeController3 node = null) {
        if (node?.parentList == siblings) return;

        nodeContainerFirst.SetPosition((0, -topPart.height));
        nodeContainerSecond.SetPosition((0, nodeContainerFirst.endPosition.y));

        SetDimensions();

        siblings.SetPosition((0, -height));

        parentList?.SetPartsPosition(this);
    }

    public override void SetDimensions() {
        height = topPart.height + nodeContainerFirst.height + nodeContainerSecond.height;
        width =
            topPart.width < (nodeContainerFirst.width + bodyPartFirst.width)
            ? nodeContainerFirst.width + bodyPartFirst.width
            : topPart.width < nodeContainerSecond.width + bodyPartSecond.width
            ? nodeContainerSecond.width + bodyPartSecond.width
            : topPart.width;
    }

}
