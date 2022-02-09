using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

[Serializable]
public struct Range {
    public int begin;
    public int end;
}


public class NodeCollider : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, INodeExpander {

    [SerializeField] NodeTransform container;

    [SerializeField] new PolygonCollider2D collider;
    [SerializeField] List<Vector2> colliderPoints;

    [SerializeField] Range widthPointsRange;
    [SerializeField] Range heightPointsRange;

    [SerializeField] NodeTransform ownTransform;

    public NodeController controller => ownTransform.controller;

    void Awake() {
        SetDefaultCollider();
    }

    void SetDefaultCollider() {
        collider.SetPath(0, colliderPoints);
    }

    public void OnPointerDown(PointerEventData eventData) {
        container.transform.SetAsLastSibling();
        HierarchyController.instance.SetOnTop(container.controller);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        controller.SetMiddleZone(true);
        controller.DetachFromParent();

        container.transform.SetAsLastSibling();
        HierarchyController.instance.SetOnTop(container.controller);

        var (dx, dy) = (eventData.delta.x, eventData.delta.y);

        container.SetFloatPositionByDelta(dx, dy);
    }

    public void OnDrag(PointerEventData eventData) {
        var (dx, dy) = (eventData.delta.x, eventData.delta.y);

        container.SetFloatPositionByDelta(dx, dy);
    }

    public void OnEndDrag(PointerEventData eventData) {
        controller.InvokeZones();
        controller.SetMiddleZone(false);
    }

    void INodeExpander.Expand(int dx, int dy, NodeTransform fromThistransform) {

        // Width
        for (var i = widthPointsRange.begin; i <= widthPointsRange.end; ++i) {
            var point = colliderPoints[i];
            point.x += dx;
            colliderPoints[i] = point;
        }

        // Height
        for (var i = heightPointsRange.begin; i <= heightPointsRange.end; ++i) {
            var point = colliderPoints[i];
            point.y -= dy;
            colliderPoints[i] = point;
        }

        collider.SetPath(0, colliderPoints);
    }
}
