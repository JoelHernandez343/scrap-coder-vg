// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.Interpreter;

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

        // Methods
        public void OnPointerDown(PointerEventData eventData) {
            HierarchyController.instance.SetOnTopOfNodes(controller);
            controller.SetState(state: "over", propagation: true);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (Executer.instance.isRunning && controller.hasParent) return;
            if (controller.ownTransform.isMovingSmoothly) return;

            previousPosition = controller.BeginDrag(eventData);

            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData) {
            if (controller.ownTransform.isMovingSmoothly) return;

            controller.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (isDragging) {
                controller.OnEndDrag(previousPosition);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {

        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            controller.SetState(state: "normal", propagation: true);
        }
    }
}