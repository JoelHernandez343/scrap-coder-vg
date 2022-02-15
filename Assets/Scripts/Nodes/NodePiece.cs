// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodePiece : MonoBehaviour, INodeExpander {

        [SerializeField] NodeTransform middleZone;
        [SerializeField] NodeTransform bottomZone;

        [SerializeField] new NodeTransform collider;
        [SerializeField] NodeTransform unionSprite;
        [SerializeField] NodeTransform middleZone;
        [SerializeField] NodeTransform bottomZone;
        [SerializeField] NodeTransform children;
        [SerializeField] NodeTransform shape;

        [SerializeField] NodeTransform ownTransform;

        public NodeController controller => ownTransform.controller;

        void INodeExpander.Expand(int dx, int dy, NodeArray toThisArray) {

            collider?.Expand(dx, dy);
            shape?.Expand(dx, dy);

            unionSprite?.SetPositionByDelta(dy: -dy);
            bottomZone?.SetPositionByDelta(dy: -dy);
            children?.SetPositionByDelta(dy: -dy);
        }
    }
}