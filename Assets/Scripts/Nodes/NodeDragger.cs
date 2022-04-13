// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {

    public class NodeDragger : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

        // Lazy and other variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        public bool isDragging {
            get => controller.isDragging;
            set => controller.isDragging = value;
        }

        Vector2Int previousPosition = Vector2Int.zero;

        public void OnPointerDown(PointerEventData eventData) {
            HierarchyController.instance.SetOnTopOfNodes(controller);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (ownTransform.isMovingSmoothly) return;

            HierarchyController.instance.SetOnTopOfCanvas(controller);

            previousPosition.x = controller.ownTransform.x;
            previousPosition.y = controller.ownTransform.y;

            controller.SetMiddleZone(true);
            controller.DetachFromParent();

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

            controller.currentDrop = controller.GetDrop();

            if (controller.currentDrop != controller.previousDrop) {
                controller.currentDrop?.SetState("over");
                controller.previousDrop?.SetState("normal");

                controller.previousDrop = controller.currentDrop;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (isDragging) {

                var dragDropZone = controller.GetDrop();

                if (dragDropZone?.category == "working") {
                    if (!controller.InvokeZones()) HierarchyController.instance.SetOnTopOfNodes(controller);
                    controller.SetMiddleZone(false);
                    dragDropZone.SetState("normal");
                } else if (dragDropZone?.category == "erasing") {
                    isDragging = false;
                    controller.Disappear();
                    dragDropZone.SetState("normal");
                    return;
                } else {
                    controller.ownTransform.SetPosition(
                        x: previousPosition.x,
                        y: previousPosition.y,
                        smooth: true,
                        endingCallback: () => HierarchyController.instance.SetOnTopOfNodes(controller)
                    );
                }

                isDragging = false;
            }
        }
    }
}