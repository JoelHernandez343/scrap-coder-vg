// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {

    public class NodeDragger :
        MonoBehaviour,
        IPointerDownHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerUpHandler {

        // Lazy and other variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        public bool isDragging {
            get => controller.isDragging;
            set => controller.isDragging = value;
        }

        Vector2Int previousPosition = Vector2Int.zero;

        bool over;

        // Methods
        public void OnPointerDown(PointerEventData eventData) {
            HierarchyController.instance.SetOnTopOfNodes(controller);
            controller.SetState("over");
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (ownTransform.isMovingSmoothly) return;

            controller.SetMiddleZone(true);
            controller.DetachFromParent();

            HierarchyController.instance.SetOnTopOfCanvas(controller);

            previousPosition.x = controller.ownTransform.x;
            previousPosition.y = controller.ownTransform.y;

            controller.ownTransform.SetFloatPositionByDelta(
                dx: eventData.delta.x,
                dy: eventData.delta.y
            );

            isDragging = true;

            controller.SetState("over");
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
                    controller.SetState("normal");
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

                controller.SetState("normal");
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            over = true;

            // controller.SetState(isDragging ? "pressed" : "over");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            over = false;

            if (!isDragging) {
                // controller.SetState("normal");
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if (!isDragging) {
                controller.SetState("normal");
            }
        }
    }
}