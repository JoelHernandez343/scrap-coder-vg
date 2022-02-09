using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePiece : MonoBehaviour, INodeExpander {

    [SerializeField] NodeTransform middleZone;
    [SerializeField] NodeTransform bottomZone;

    [SerializeField] new NodeTransform collider;
    [SerializeField] NodeTransform unionSprite;
    [SerializeField] NodeTransform shape;

    [SerializeField] NodeTransform ownTransform;

    public NodeController controller => ownTransform.controller;

    public void SetPosition((int x, int y) position) {
        ownTransform.SetPosition(position);
    }

    public void SetPositionByDelta(int dx = 0, int dy = 0) {
        ownTransform.SetPositionByDelta(dx, dy);
    }

    void INodeExpander.Expand(int dx, int dy, NodeTransform fromThistransform) {

        shape?.Expand(dx, dy);
        collider?.Expand(dx, dy);
        unionSprite?.SetPositionByDelta(dy: -dy);

        bottomZone?.SetPositionByDelta(dy: -dy);
    }
}
