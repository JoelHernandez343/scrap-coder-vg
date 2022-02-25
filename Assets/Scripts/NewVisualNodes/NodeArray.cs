using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeArray : MonoBehaviour {

    [SerializeField] public NodeController controller;
    [SerializeField] public NodeTransform nodeTransform;

    [SerializeField] public List<NodeController> nodes;

    int borderOffset = 1;

    public int Count => nodes.Count;

    public NodeController this[int index] => nodes[index];

    public NodeController Last => Count == 0 ? null : nodes[Count - 1];

    public int width => nodeTransform.width;
    public int height => nodeTransform.height;

    public int x => nodeTransform.x;
    public int y => nodeTransform.y;
    public int fx => nodeTransform.fx;
    public int fy => nodeTransform.fy;

    public int initWidth => nodeTransform.initWidth;
    public int initHeight => nodeTransform.initHeight;

    public (int x, int y) position => nodeTransform.position;
    public (int fx, int fy) finalPosition => nodeTransform.finalPosition;

    List<NodeController> RemoveNodes(NodeController fromThisNode = null) {

        if (Count == 0) return new List<NodeController>();

        var lowerIndex =
            fromThisNode != null
            ? nodes.IndexOf(fromThisNode)
            : 0;

        var count = Count - lowerIndex;
        var removedNodes = nodes.GetRange(lowerIndex, count);
        nodes.RemoveRange(lowerIndex, count);

        controller.RefreshZones(array: this, node: Last);
        SetPartsPosition(Last);


        return removedNodes;
    }

    public void AddNodes(NodeController node, NodeController toThisNode = null) {
        var newNodes = node.siblings.RemoveNodes();
        newNodes.Insert(0, node);

        AddRangeOfNodes(newNodes, toThisNode);
    }

    public void AddNodesFromParent() {
        var newNodes = controller.parentArray.RemoveNodes(controller);
        newNodes.RemoveAt(0);

        controller.ClearParent();

        AddRangeOfNodes(newNodes, controller);
    }

    void AddRangeOfNodes(List<NodeController> newNodes, NodeController toThisNode) {
        if (newNodes.Count == 0) {
            controller.RefreshZones(array: this);
            return;
        }

        // Update hierarchy parent
        newNodes.ForEach(node => node.parentArray = this);

        // Add to the nodes list right after fromThisNode
        var index = toThisNode != controller
            ? nodes.IndexOf(toThisNode) + 1
            : 0;

        nodes.InsertRange(index, newNodes);

        controller.RefreshZones(array: this, node: newNodes[0]);
        SetPartsPosition(newNodes[0]);
    }

    public void RefreshNodeZones(NodeController node = null) {
        if (node == null) {
            Debug.Assert(Count == -0, controller.gameObject.name);
            return;
        }

        var begin = nodes.IndexOf(node);

        for (var i = begin; i < Count; ++i) {
            if (nodes[i] != Last) {
                nodes[i].SetZones(SetZone.asChild);
            } else {
                nodes[i].SetZones(SetZone.asLastChild);
            }
        }
    }

    public void SetPartsPosition(NodeController node) {
        if (node == null) {
            nodeTransform.ExpandByNewDimensions(0, 0);
            controller.SetPartsPosition(this);
            return;
        }

        var anchor = (x: 0, y: 0 + borderOffset);
        var maxWidth = 0;
        var begin = nodes.IndexOf(node);

        if (begin > 0) {
            anchor.x = nodes[begin - 1].nodeTransform.x;
            anchor.y = nodes[begin - 1].nodeTransform.fy + borderOffset;
        }

        for (var i = begin; i < Count; ++i) {
            nodes[i].nodeTransform.SetPosition(anchor);
            anchor.y = nodes[i].nodeTransform.fy + borderOffset;
        }

        nodes.ForEach(n => maxWidth =
            maxWidth < n.nodeTransform.width
            ? n.nodeTransform.width
            : maxWidth
        );

        nodeTransform.ExpandByNewDimensions(maxWidth, -anchor.y);

        controller.SetPartsPosition(this);
    }

}