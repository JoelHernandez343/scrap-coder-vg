using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePiece : MonoBehaviour, INodeExpander {

    // [SerializeField] NodeZone topZone;
    [SerializeField] NodeTransform middleZone;
    [SerializeField] NodeTransform bottomZone;

    [SerializeField] NodeTransform unionSprite;

    [SerializeField] NodeTransform mainShape;

    [SerializeField] new NodeTransform collider;

    [SerializeField] NodeTransform nodeTransform;

    public void SetPosition((int x, int y) position) {
        nodeTransform.SetPosition(position);
    }

    public void SetPositionByDelta(int dx = 0, int dy = 0) {
        nodeTransform.SetPositionByDelta(dx, dy);
    }

    void INodeExpander.Expand(int dx, int dy) {

        mainShape?.Expand(dx, dy);
        collider?.Expand(dx, dy);
        unionSprite?.SetPositionByDelta(dy: -dy);

        bottomZone?.SetPositionByDelta(dy: -dy);
    }
}
