// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {

    public class NodeCollider : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, INodeExpander {

        // Internal types
        [System.Serializable]
        struct NodeRange {
            public int begin;
            public int end;
        }

        // Editor variables
        [SerializeField] new PolygonCollider2D collider;

        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        [SerializeField] NodeTransform ownTransform;

        // State variables
        [SerializeField] List<Vector2> colliderPoints;

        // Lazy and other variables
        public NodeController controller => ownTransform.controller;

        // Methods
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
            if (eventData.dragging) {
                var (dx, dy) = (eventData.delta.x, eventData.delta.y);

                controller.ownTransform.SetFloatPositionByDelta(dx, dy);
            }
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