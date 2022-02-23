// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace ScrapCoder.VisualNodes {

    [Serializable]
    public struct NodeRange {
        public int begin;
        public int end;
    }

    public class NodeCollider : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, INodeExpander {

        [SerializeField] new PolygonCollider2D collider;
        [SerializeField] List<Vector2> colliderPoints;

        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        [SerializeField] NodeTransform ownTransform;

        public NodeController controller => ownTransform.controller;

        void Awake() {
            SetDefaultCollider();
        }

        void SetDefaultCollider() {
            collider.SetPath(0, colliderPoints);
        }

        public void OnPointerDown(PointerEventData eventData) {
            HierarchyController.instance.SetOnTop(controller);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            controller.SetMiddleZone(true);
            controller.DetachFromParent();

            HierarchyController.instance.SetOnTop(controller);

            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            controller.ownTransform.SetFloatPositionByDelta(dx, dy);
        }

        public void OnDrag(PointerEventData eventData) {
            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            controller.ownTransform.SetFloatPositionByDelta(dx, dy);
        }

        public void OnEndDrag(PointerEventData eventData) {
            controller.InvokeZones();
            controller.SetMiddleZone(false);
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray _) {

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

            return (dx, dy);
        }
    }
}