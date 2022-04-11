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

        Vector2Int previousPosition = Vector2Int.zero;

        public void OnPointerDown(PointerEventData eventData) {
            HierarchyController.instance.SetOnTop(controller);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (ownTransform.isMovingSmoothly) return;

            previousPosition.x = controller.ownTransform.x;
            previousPosition.y = controller.ownTransform.y;

            controller.SetMiddleZone(true);
            controller.DetachFromParent();

            HierarchyController.instance.SetOnTop(controller);

            controller.ownTransform.SetFloatPositionByDelta(
                dx: eventData.delta.x,
                dy: eventData.delta.y
            );

            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData) {
            if (ownTransform.isMovingSmoothly) return;

            if (eventData.dragging && isDragging) {
                controller.ownTransform.SetFloatPositionByDelta(
                    dx: eventData.delta.x,
                    dy: eventData.delta.y
                );
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (isDragging) {
                if (!controller.InvokeZones() && !UI.WorkingZone.instance.IsOnWorkingZone(controller)) {
                    controller.ownTransform.SetPosition(
                        x: previousPosition.x,
                        y: previousPosition.y,
                        smooth: true
                    );
                } else {
                    controller.SetMiddleZone(false);
                }

                isDragging = false;
            }
        }
    }
}