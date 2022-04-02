// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeContainer : MonoBehaviour {

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

        // Methods
        public void Clear() {
            array.RefreshNodeZones(array[0]);
        }

        public void AdjustParts((int dx, int dy) delta, bool smooth = false) {
            var newDelta = CalculateDelta(delta);

            RecalculateZLevels();

            if (toggleZone) zone?.SetActive(array.Count == 0);
            sprite?.ToggleRender(array.Count == 0);
            ownTransform.Expand(dx: newDelta.dx, dy: newDelta.dy, smooth: smooth);
            controller.AdjustParts(array, newDelta, smooth: smooth);
        }

        (int dx, int dy) CalculateDelta((int dx, int dy) delta) {
            if (array.previousCount == 0) {
                delta.dy -= defaultHeight;
                delta.dx -= defaultWidth;
            } else if (array.Count == 0) {
                delta.dy += defaultHeight;
                delta.dx += defaultWidth;
            }

            return delta;
        }

        void RecalculateZLevels() {
            ownTransform.maxZlevels = array.ownTransform.zLevels;
        }

        public void AddNodes(NodeController nodeToAdd, NodeController toThisNode = null, bool smooth = false) {
            toThisNode ??= controller;

            if (acceptedCategory == NodeCategory.All || acceptedCategory == nodeToAdd.category) {
                array.AddNodes(node: nodeToAdd, toThisNode: toThisNode, smooth: smooth);
            }
        }
    }
}