// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Interpreter;

namespace ScrapCoder.VisualNodes {

    public class NodeContainer : MonoBehaviour, INodeExpanded {

        // Editor variables
        [SerializeField] public NodeZone zone;
        [SerializeField] public NodeArray array;
        [SerializeField] public NodeSprite sprite;

        [SerializeField] bool toggleZone = false;

        [SerializeField] public NodeTransform pieceToExpand;
        [SerializeField] public bool modifyWidthOfPiece;
        [SerializeField] public bool modifyHeightOfPiece;

        [SerializeField] public int defaultHeight;
        [SerializeField] public int defaultWidth;

        [SerializeField] public NodeCategory acceptedCategory;

        // Lazy and other variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        public int Count => array.Count;

        public NodeController Last => array.Last;

        public bool isEmpty => array.Count == 0;

        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;
        bool INodeExpanded.ModifyHeightOfPiece => modifyHeightOfPiece;
        bool INodeExpanded.ModifyWidthOfPiece => modifyWidthOfPiece;

        // Methods
        public void Clear() {
            array.RefreshNodeZones(array[0]);
        }

        public void AdjustParts(int? dx = null, int? dy = null, bool smooth = false) {
            (dx, dy) = CalculateDelta(dx: dx, dy: dy);

            RefreshLocalDepthLevels();

            if (toggleZone) zone?.SetActive(array.Count == 0);
            sprite?.SetVisible(array.Count == 0);
            ownTransform.Expand(dx: dx, dy: dy, smooth: smooth);

            ownTransform.expandable.Expand(dx: dx, dy: dy, smooth: smooth, expanded: this);
        }

        (int? dx, int? dy) CalculateDelta(int? dx, int? dy) {
            dx ??= 0;
            dy ??= 0;

            if (array.previousCount == 0) {
                dy -= defaultHeight;
                dx -= defaultWidth;
            } else if (array.Count == 0) {
                dy += defaultHeight;
                dx += defaultWidth;
            }

            return (dx, dy);
        }

        void RefreshLocalDepthLevels() {
            ownTransform.localDepthLevels = array.ownTransform.depthLevels;
        }

        public void AddNodes(NodeController nodeToAdd, NodeController toThisNode = null, bool smooth = false) {
            toThisNode ??= controller;

            if (acceptedCategory == NodeCategory.All || acceptedCategory == nodeToAdd.category) {
                array.AddNodes(node: nodeToAdd, toThisNode: toThisNode, smooth: smooth);
            }
        }

        public void SetState(string state) {
            array.nodes.ForEach(n => n.SetState(state));
        }

        public void RemoveNodesFromTableSymbol() {
            array.nodes.ForEach(n => n.RemoveFromSymbolTable());
        }

        public bool Analyze() {
            foreach (var n in array.nodes) {
                if (!n.Analyze()) return false;
            }

            return true;
        }
    }
}