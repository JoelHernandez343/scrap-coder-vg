// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeArray : MonoBehaviour {

        // Editor variables
        [SerializeField] public NodeContainer container;

        // State Variables
        [System.NonSerialized] public List<NodeController> nodes = new List<NodeController>();
        public int previousCount { get; private set; }

        // Lazy and other variables
        int borderOffset = 1;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

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

        Utils.SmoothDampController auxiliarSmoothDamp = new Utils.SmoothDampController(0.1f);

        // Methods
        void FixedUpdate() {
            if (auxiliarSmoothDamp.isWorking) UpdateSmoothDamp();
        }

        void UpdateSmoothDamp() {
            var (_, endingCallback) = auxiliarSmoothDamp.NextDelta();

            // If finished, execute callback
            if (auxiliarSmoothDamp.isFinished && endingCallback != null) {
                endingCallback();
            }
        }

        public List<NodeController> RemoveNodes(NodeController fromThisNode = null, bool smooth = false) {

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
            AdjustParts(previousCount: previousCount, smooth: smooth);

            return removedNodes;
        }

        public void AddNodes(NodeController node, NodeController toThisNode = null, bool smooth = false) {
            var newNodes = node.siblings?.RemoveNodes(smooth: smooth) ?? new List<NodeController>();
            newNodes.Insert(0, node);

            AddRangeOfNodes(newNodes, toThisNode, smooth: smooth);
        }

        public void AddNodesFromParent(bool smooth = false) {
            var newNodes = controller.parentArray.RemoveNodes(controller, smooth: smooth);
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
            newNodes.ForEach(node => node.parentArray = this);

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

            // Set the changed node on top others
            changedNode.ownTransform.sorter.sortingOrder = 2;

            // Move down the nest siblings
            if (changedNode != Last) {
                var nodeIndex = nodes.IndexOf(changedNode);

                MoveChunkOfNodesByDelta(
                    startIndex: nodeIndex + 1,
                    endIndex: Count - 1,
                    dy: -dy,
                    smooth: smooth
                );
            }
            if (smooth) {
                // If smooth, add callback when changedNode finish expanding to reverse top
                auxiliarSmoothDamp.AddDeltaToDestination(
                    dy: dy,
                    endingCallback: (() => changedNode.ownTransform.sorter.sortingOrder = 0)
                );
            } else {
                changedNode.ownTransform.sorter.sortingOrder = 0;
            }

            Adjust(dy, smooth: smooth);
        }

        // Adjust when remove nodes
        void AdjustParts(int? previousCount = null, bool smooth = false) {
            this.previousCount = previousCount ?? Count;

            int previousY = (Last?.ownTransform.fy + borderOffset) ?? 0;
            int dy = -previousY - ownTransform.height;

            Adjust(dy, smooth: smooth);
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

            var changeOfPosition = MoveChunkOfNodes(
                startIndex: firstIndex,
                endIndex: lastIndex,
                x: 0,
                y: previousY,
                smooth: smooth
            );

            var maxChange = changeOfPosition;

            if (lastNode != Last) {
                var changeOfScrollDown = MoveChunkOfNodesByDelta(
                    startIndex: lastIndex + 1,
                    endIndex: Count - 1,
                    dy: -dy,
                    smooth: smooth
                );

                // Calculate the max change in order to add callback for order nodes
                if (changeOfScrollDown.magnitude > changeOfPosition.magnitude) {
                    maxChange = changeOfScrollDown;
                }
            }

            if (smooth) {
                // When the last change finish, then order nodes
                auxiliarSmoothDamp.AddDeltaToDestination(
                    dx: (int)System.Math.Round(maxChange.x),
                    dy: (int)System.Math.Round(maxChange.y),
                    endingCallback: () => OrderNodes()
                );
            } else {
                OrderNodes();
            }


            Adjust(dy - (firstIndex == 0 && lastNode == Last ? borderOffset : 0), smooth: smooth);
        }

        Vector2 MoveChunkOfNodes(int startIndex, int endIndex, int? x = null, int? y = null, bool smooth = false) {
            return MoveChunk(startIndex, endIndex, x, y, smooth, false);
        }

        Vector2 MoveChunkOfNodesByDelta(int startIndex, int endIndex, int? dx = null, int? dy = null, bool smooth = false) {
            return MoveChunk(startIndex, endIndex, dx, dy, smooth, true);
        }

        Vector2 MoveChunk(int startIndex, int endIndex, int? x, int? y, bool smooth, bool isByDelta) {
            var (owner, ownership) = SetOwnership(startIndex, endIndex + 1);

            System.Action endingCallback = () => RevertOwnership(ownership);

            Vector2 change;

            if (isByDelta) {
                change = owner.ownTransform.SetPositionByDelta(
                    dx: x,
                    dy: y,
                    smooth: smooth,
                    endingCallback: smooth ? endingCallback : (System.Action)null
                );
            } else {
                change = owner.ownTransform.SetPosition(
                    x: x,
                    y: y,
                    smooth: smooth,
                    endingCallback: smooth ? endingCallback : (System.Action)null
                );
            }

            if (!smooth) {
                endingCallback();
            }

            return change;
        }

        void Adjust(int? dy = null, bool smooth = false) {
            int dx = currentMaxWidth - ownTransform.width;

            ownTransform.Expand(dx: dx, dy: dy);
            RefreshLocalDepthLevels();
            container.AdjustParts(dx: dx, dy: dy, smooth: smooth);
        }

        void RefreshLocalDepthLevels() {
            var localDepthLevels = 0;
            foreach (var node in nodes) {
                var tf = node.ownTransform;

                if (localDepthLevels < tf.depthLevels) {
                    localDepthLevels = tf.depthLevels;
                }
            }

            ownTransform.localDepthLevels = localDepthLevels;
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

            ownership.Insert(0, owner);

            return (owner, ownership);
        }

        void RevertOwnership(List<NodeController> nodes) {
            nodes.ForEach(node => {
                node.parentArray = this;
                node.ownTransform.RefreshPosition();
                node.ownTransform.ResetRenderOrder();
            });
        }

        void OrderNodes() {
            nodes.ForEach(node => {
                node.ownTransform.ResetRenderOrder();
                node.ownTransform.rectTransform.SetAsLastSibling();
            });
        }

        public void SetStateAfterThis(NodeController node, string state) {
            bool passed = false;

            foreach (var n in nodes) {
                if (n == node) {
                    passed = true;
                    continue;
                } else if (!passed) {
                    continue;
                };

                n.SetState(state);
            }
        }
    }
}