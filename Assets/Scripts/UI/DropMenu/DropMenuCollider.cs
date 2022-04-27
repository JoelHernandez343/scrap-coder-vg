// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class DropMenuCollider :
        MonoBehaviour,
        IPointerDownHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerUpHandler {

        // Editor variables
        [SerializeField] DropMenuController dropMenu;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        bool hasFocus => (dropMenu as InputManagment.IFocusable).HasFocus();

        bool hasController => controller != null;

        bool isDragging {
            get => controller.isDragging;
            set => controller.isDragging = value;
        }

        Vector2Int previousPosition = Vector2Int.zero;

        public void OnPointerDown(PointerEventData eventData) {
            if (!hasController) return;

            if (hasFocus) return;

            HierarchyController.instance.SetOnTopOfNodes(controller);
            controller.SetState("over");
        }

        public void OnBeginDrag(PointerEventData e) {
            if (!hasController) return;

            if (hasFocus) return;
            if (Executer.instance.isRunning) return;
            if (ownTransform.isMovingSmoothly) return;

            previousPosition = controller.BeginDrag(e);

            isDragging = true;
        }

        public void OnDrag(PointerEventData e) {
            if (!hasController) return;

            if (hasFocus) return;
            if (Executer.instance.isRunning) return;
            if (controller.ownTransform.isMovingSmoothly) return;

            controller.OnDrag(e);
        }

        public void OnEndDrag(PointerEventData e) {
            if (!hasController) return;

            if (isDragging) {
                controller.OnEndDrag(previousPosition);
            }
        }

        public void OnPointerUp(PointerEventData e) {
            if (!hasController) return;

            if (!isDragging && !hasFocus) {
                controller.SetState("normal");
            }
        }
    }
}