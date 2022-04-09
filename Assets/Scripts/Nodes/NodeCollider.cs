// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {

    public class NodeCollider : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

        // Lazy and other variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

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
    }
}