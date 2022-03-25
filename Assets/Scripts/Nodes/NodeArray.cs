// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeArray : MonoBehaviour {

        // Editor variables
        [SerializeField] public NodeTransform ownTransform;
        [SerializeField] public NodeContainer container;

        // State Variables
        [System.NonSerialized] public List<NodeController> nodes = new List<NodeController>();
        public int previousCount { get; private set; }

        // Lazy and other variables
        int borderOffset = 1;

        public NodeController controller => ownTransform.controller;

        public int Count => nodes.Count;

        public NodeController this[int index] => nodes[index];
        public NodeController Last => Count == 0 ? null : nodes[Count - 1];

        bool acceptEnd => controller.siblings == this;

        int currentMaxWidth {
            get {
                var maxWidth = 0;
                nodes.ForEach(node => maxWidth =
                    maxWidth < node.ownTransform.width
                        ? node.ownTransform.width
                        : maxWidth
                );

                return maxWidth;
            }
        }

        // Methods
        public List<NodeController> RemoveNodes(NodeController fromThisNode = null) {

            if (Count == 0) return new List<NodeController>();

            var previousCount = Count;

            var lowerIndex =
                fromThisNode != null
                ? nodes.IndexOf(fromThisNode)
                : 0;

            var count = Count - lowerIndex;
            var removedNodes = nodes.GetRange(lowerIndex, count);
            nodes.RemoveRange(lowerIndex, count);

            // Remove this nodes from hierarchy avoiding unwanted repositions
            removedNodes.ForEach(node => node.ClearParent());

            controller.RefreshZones(array: this, node: Last);
            AdjustParts(previousCount: previousCount);

            return removedNodes;
        }

        public void AddNodes(NodeController node, NodeController toThisNode = null) {
            var newNodes = node.siblings?.RemoveNodes() ?? new List<NodeController>();
            newNodes.Insert(0, node);

            AddRangeOfNodes(newNodes, toThisNode, smooth: true);
        }

        public void AddNodesFromParent() {
            var newNodes = controller.parentArray.RemoveNodes(controller);
            newNodes.RemoveAt(0);

            controller.ClearParent();

            AddRangeOfNodes(newNodes, controller, smooth: false);
        }

        void AddRangeOfNodes(List<NodeController> newNodes, NodeController toThisNode, bool smooth = false) {
            if (newNodes.Count == 0) {
                controller.RefreshZones(array: this);
                return;
            }

            var previousCount = Count;

            // Update hierarchy parent
            newNodes.ForEach(node => {
                node.parentArray = this;
                node.ownTransform.ResetZPosition();
            });

            // Add to the nodes list right after fromThisNode
            var index = toThisNode != controller
                ? nodes.IndexOf(toThisNode) + 1
                : 0;

            RemoveEndNodes(newNodes, index);

            if (newNodes.Count == 0) {
                controller.RefreshZones(array: this);
                return;
            }

            nodes.InsertRange(index, newNodes);

            controller.RefreshZones(array: this, node: newNodes[0]);
            AdjustParts(newNodes: newNodes, smooth: smooth, previousCount: previousCount);
        }

        public void RefreshNodeZones(NodeController node = null) {
            if (node == null) return;

            var begin = nodes.IndexOf(node);

            if (begin > 0) {
                nodes[begin - 1].SetZones(SetZone.asChild);
            }

            for (var i = begin; i < Count; ++i) {
                if (nodes[i] != Last) {
                    nodes[i].SetZones(SetZone.asChild);
                } else {
                    nodes[i].SetZones(SetZone.asLastChild);
                }
            }
        }

        // Adjust when one node is changed
        public void AdjustParts(NodeController changedNode, int? dx = null, int? dy = null, bool smooth = false) {
            this.previousCount = Count;

            if (changedNode == Last) {
                Adjust(dy);
                return;
            }

            var nodeIndex = nodes.IndexOf(changedNode);
            var (afterNode, afterNodes) = SetOwnership(nodeIndex + 1, Count);

            afterNode.ownTransform.SetPositionByDelta(
                dy: -dy,
                smooth: smooth,
                endingCallBack: BuildCallBack(afterNodes, smooth)
            );
            if (!smooth) RevertOwnership(afterNodes);

            Adjust(dy);
        }

        // Adjust when remove nodes
        void AdjustParts(int? previousCount = null) {
            this.previousCount = previousCount ?? Count;

            int previousY = (Last?.ownTransform.fy + borderOffset) ?? 0;
            int dy = -previousY - ownTransform.height;

            Adjust(dy);
        }

        // Adjust when adding nodes
        void AdjustParts(List<NodeController> newNodes, bool smooth = false, int? previousCount = null) {
            this.previousCount = previousCount ?? Count;

            var firstNode = newNodes[0];
            var lastNode = newNodes[newNodes.Count - 1];

            var firstIndex = nodes.IndexOf(firstNode);
            var lastIndex = nodes.IndexOf(lastNode);

            var previousY = firstIndex == 0 ? borderOffset : nodes[firstIndex - 1].ownTransform.fy + borderOffset;

            var dy = 0;
            newNodes.ForEach(node => dy += node.ownTransform.height - borderOffset);

            var (newNodesParent, newNodesOwnership) = SetOwnership(firstIndex, lastIndex + 1);
            newNodesParent.ownTransform.SetPosition(
                x: 0,
                y: previousY,
                smooth: smooth,
                endingCallBack: BuildCallBack(newNodesOwnership, smooth)
            );
            if (!smooth) RevertOwnership(newNodesOwnership);

            if (lastNode != Last) {
                var (afterNode, afterNodes) = SetOwnership(lastIndex + 1, Count);
                afterNode.ownTransform.SetPositionByDelta(
                    dy: -dy,
                    smooth: smooth,
                    endingCallBack: BuildCallBack(afterNodes, smooth)
                );
                if (!smooth) RevertOwnership(afterNodes);
            }

            Adjust(dy - (firstIndex == 0 && lastNode == Last ? borderOffset : 0));
        }

        void Adjust(int? dy = null) {
            OrderNodes();

            int dx = currentMaxWidth - ownTransform.width;

            ownTransform.Expand(dx, dy ?? 0);
            RecalculateZLevels();
            container.AdjustParts((dx, dy ?? 0));
        }

        void RecalculateZLevels() {
            var maxZlevels = 0;
            foreach (var node in nodes) {
                var tf = node.ownTransform;
                maxZlevels = tf.zLevels < maxZlevels
                    ? tf.zLevels
                    : maxZlevels;
            }

            ownTransform.maxZlevels = maxZlevels;
        }

        void RemoveEndNodes(List<NodeController> newNodes, int indexToInsert) {
            var areInsertedToEnd = indexToInsert == Count;

            for (var i = 0; i < newNodes.Count; ++i) {
                var node = newNodes[i];

                if (node.type == NodeType.End) {
                    if (areInsertedToEnd && i == newNodes.Count - 1 && acceptEnd) break;

                    newNodes.RemoveAt(i);
                    node.Eject();
                    i--;
                }
            }
        }

        (NodeController owner, List<NodeController> ownership) SetOwnership(int start, int end) {
            var owner = nodes[start];
            var ownership = new List<NodeController>();

            for (var i = start + 1; i < end; ++i) {
                nodes[i].temporalParent = owner;
                ownership.Add(nodes[i]);
            }

            return (owner, ownership);
        }

        void RevertOwnership(List<NodeController> nodes) {
            nodes.ForEach(node => {
                node.parentArray = this;
                node.ownTransform.ResetZPosition();
                node.ownTransform.RefreshPosition();
            });
        }

        System.Action BuildCallBack(List<NodeController> nodes, bool smooth) {
            if (!smooth) return null;

            System.Action cb = () => {
                RevertOwnership(nodes);
                OrderNodes();
            };

            return cb;
        }

        void OrderNodes() {
            nodes.ForEach(node => node.ownTransform.rectTransform.SetAsLastSibling());
        }
    }
}