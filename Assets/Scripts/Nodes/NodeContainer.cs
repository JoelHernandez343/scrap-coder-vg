// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeContainer : MonoBehaviour {
        [SerializeField] public NodeZone zone;
        [SerializeField] public NodeArray array;
        [SerializeField] public NodeSprite sprite;

        [SerializeField] public NodePiece pieceToExpand;
        [SerializeField] public bool modifyWidthOfPiece;
        [SerializeField] public bool modifyHeightOfPiece;

        [SerializeField] public int defaultHeight;
        [SerializeField] public int defaultWidth;

        [SerializeField] public NodeTransform ownTransform;

        public NodeController controller => ownTransform.controller;

        public int Count => array.Count;

        public void Clear() {
            array.RefreshNodeZones(array[0]);
        }

        public void AdjustParts((int dx, int dy) delta) {
            sprite?.toggleRender(array.Count == 0);
            controller.AdjustParts(array, delta);
        }
    }
}