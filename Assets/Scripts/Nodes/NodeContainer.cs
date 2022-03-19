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

        [SerializeField] public NodeTransform ownTransform;

        // Lazy and other variables
        public NodeController controller => ownTransform.controller;

        public int Count => array.Count;

        // Methods
        public void Clear() {
            array.RefreshNodeZones(array[0]);
        }

        public void AdjustParts((int dx, int dy) delta) {
            var newDelta = CalculateDelta(delta);

            RecalculateZLevels();

            if (toggleZone) zone?.SetActive(array.Count == 0);
            sprite?.ToggleRender(array.Count == 0);
            ownTransform.Expand(newDelta.dx, newDelta.dy);
            controller.AdjustParts(array, newDelta);
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

        public void AddNodes(NodeController nodeToAdd, NodeController toThisNode = null) {
            toThisNode ??= controller;

            if (acceptedCategory == NodeCategory.All || acceptedCategory == nodeToAdd.type) {
                array.AddNodes(nodeToAdd, toThisNode);
            }
        }
    }
}