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

    [SerializeField] public new PolygonCollider2D collider;
    [SerializeField] List<Vector2> colliderPoints;
    [SerializeField] NodeController controller;

    [SerializeField] Range widthPointsRange;
    [SerializeField] Range heightPointsRange;

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

    void INodeExpander.Expand(int dx, int dy) {

        var newPath = new List<Vector2>(collider.GetPath(0));

        // Width
        for (var i = widthPointsRange.begin; i <= widthPointsRange.end; ++i) {
            var vector = newPath[i];
            vector.x += dx;
            newPath[i] = vector;
        }

        // Height
        for (var i = heightPointsRange.begin; i <= heightPointsRange.end; ++i) {
            var vector = newPath[i];
            vector.y -= dy;
            newPath[i] = vector;
        }

        collider.SetPath(0, newPath);
    }
}
