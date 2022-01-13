/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeList {

    public List<NodeController3> nodes = new List<NodeController3>();
    public NodeController3 controller;
    public NodeContainer container;

    public int Count {
        get => nodes.Count;
    }

    public NodeController3 this[int index] {
        get => nodes[index];
    }

    public NodeController3 Last {
        get => Count == 0 ? null : nodes[Count - 1];
    }

    public NodeList(NodeController3 controller) {
        this.controller = controller;
    }

    public (float x, float y) position { private set; get; }

    [SerializeField] public float height = 0;
    [SerializeField] public float width = 0;

    public (float x, float y) endPosition => (position.x + width, position.y - height);

    List<NodeController3> RemoveNodes(NodeController3 fromThisNode = null) {
        if (nodes.Count == 0) {
            return new List<NodeController3>();
        }

        var lowerIndex =
            fromThisNode != null
            ? nodes.FindIndex(child => child == fromThisNode)
            : 0;

        var count = nodes.Count - lowerIndex;
        var removedNodes = nodes.GetRange(lowerIndex, count);
        nodes.RemoveRange(lowerIndex, count);

        controller.Refresh(this);
        SetPartsPosition(Last);

        return removedNodes;
    }

    public void AddNodes(NodeController3 node, NodeController3 fromThisNode = null) {
        // Get all siblings and add node itself
        var newNodes = node.siblings.RemoveNodes();
        newNodes.Insert(0, node);

        AddRangeOfNodes(newNodes, fromThisNode);
    }

    public void AddNodesFromParent() {
        // Get siblings below controller and remove itself
        var newNodes = controller.parentList.RemoveNodes(controller);
        newNodes.RemoveAt(0);

        controller.ClearParent();

        AddRangeOfNodes(newNodes, controller);
    }

    void AddRangeOfNodes(List<NodeController3> newNodes, NodeController3 fromThisNode) {
        if (newNodes.Count == 0) {
            controller.Refresh(this);
            return;
        }

        // Update hierarchy parent
        newNodes.ForEach(node => node.parentList = this);

        // Add to the nodes list right after fromThisNode
        var index = fromThisNode != controller
            ? nodes.FindIndex(child => child == fromThisNode) + 1
            : 0;

        nodes.InsertRange(index, newNodes);

        controller.Refresh(this);
        SetPartsPosition(newNodes[0]);
    }

    public void SetPartsPosition(NodeController3 node = null) {
        var anchor = position;
        var maxWidth = 0f;

        if (node == null) {
            nodes.ForEach(n => anchor.y = n.SetPosition(anchor).y);
        } else {
            var begin = nodes.IndexOf(node);

            if (begin > 0) {
                anchor.x = nodes[begin - 1].position.x;
                anchor.y = nodes[begin - 1].endPosition.y;
            }

            for (var i = begin; i < nodes.Count; ++i) {
                anchor.y = nodes[i].SetPosition(anchor).y;
            }

            nodes.ForEach(n => maxWidth = maxWidth < n.width ? n.width : maxWidth);
        }

        height = position.y - anchor.y;
        width = maxWidth;

        if (container != null) {
            container.SetPartsPosition(node);
        } else {
            controller.SetPartsPosition(node);
        }
    }

    public (float x, float y) SetPosition((float x, float y) position, bool init = false) {
        if (this.position == position) {
            return endPosition;
        }

        this.position = position;

        if (!init) {
            SetPartsPosition();
        }

        return endPosition;
    }

}
