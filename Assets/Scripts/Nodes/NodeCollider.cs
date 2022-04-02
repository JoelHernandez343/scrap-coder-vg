// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {

    public class NodeCollider : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, INodeExpander {

        // Editor variables
        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        // Lazy and other variables
        PolygonCollider2D _polygonCollider;
        PolygonCollider2D polygonCollider => _polygonCollider ??= GetComponent<PolygonCollider2D>();

        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        List<NodeRange> _ranges;
        List<NodeRange> ranges
            => _ranges ??= new List<NodeRange> { widthPointsRange, heightPointsRange };

        List<Vector2> _colliderPoints;
        List<Vector2> colliderPoints
            => _colliderPoints ??= new List<Vector2>(polygonCollider.GetPath(0));

        bool isDragging = false;

        public void OnPointerDown(PointerEventData eventData) {
            HierarchyController.instance.SetOnTop(controller);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (ownTransform.isMovingSmoothly) return;

            controller.SetMiddleZone(true);
            controller.DetachFromParent();

            HierarchyController.instance.SetOnTop(controller);

            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            controller.ownTransform.SetFloatPositionByDelta(dx, dy);

            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData) {
            if (ownTransform.isMovingSmoothly) return;

            if (eventData.dragging && isDragging) {
                var (dx, dy) = (eventData.delta.x, eventData.delta.y);

                controller.ownTransform.SetFloatPositionByDelta(dx, dy);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (isDragging) {
                controller.InvokeZones();
                controller.SetMiddleZone(false);

                isDragging = false;
            }
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, NodeArray _) {
            int[] delta = { dx, dy };

            for (int axis = 0; axis < ranges.Count; ++axis) {
                var range = ranges[axis];
                var isExpandable = range.isExpandable;

                var sign = axis == 0 ? 1 : -1;

                if (!isExpandable) continue;

                for (var i = range.begin; i <= range.end; ++i) {
                    var point = colliderPoints[i];
                    point[axis] += (sign) * delta[axis];
                    colliderPoints[i] = point;
                }
            }

            polygonCollider.SetPath(0, colliderPoints);

            return (dx, dy);
        }
    }
}