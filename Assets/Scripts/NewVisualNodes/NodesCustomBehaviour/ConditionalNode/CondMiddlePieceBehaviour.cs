using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondMiddlePieceBehaviour : MonoBehaviour, INodeExpander {

    [SerializeField] NodeTransform asideShape;
    [SerializeField] NodeTransform unionSprite;
    [SerializeField] NodeTransform mainShape;
    [SerializeField] NodeTransform bottomZone;
    [SerializeField] new NodeTransform collider;

    void INodeExpander.Expand(int dx, int dy) {
        asideShape.Expand(dx, dy);
        collider.Expand(dx, dy);

        mainShape.SetFloatPositionByDelta(dy: -dy);
        unionSprite.SetFloatPositionByDelta(dy: -dy);
        bottomZone.SetFloatPositionByDelta(dy: -dy);
    }
}
