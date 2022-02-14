using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeMiddleNestingPieceBehaviour : MonoBehaviour, INodeExpander {
        [SerializeField] NodeTransform edgeShape;
        [SerializeField] NodeTransform unionSprite;
        [SerializeField] NodeTransform mainShape;
        [SerializeField] NodeTransform bottomZone;
        [SerializeField] NodeTransform children;
        [SerializeField] NodeTransform text;
        [SerializeField] new NodeTransform collider;

        void INodeExpander.Expand(int dx, int dy) {
            collider.Expand(dx, dy);
            edgeShape.Expand(dx, dy);

            text?.SetPositionByDelta(dy: -dy);
            children?.SetPositionByDelta(dy: -dy);
            mainShape?.SetPositionByDelta(dy: -dy);
            bottomZone?.SetPositionByDelta(dy: -dy);
            unionSprite?.SetPositionByDelta(dy: -dy);
        }
    }

}