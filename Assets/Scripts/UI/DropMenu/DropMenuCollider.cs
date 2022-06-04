// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;
using ScrapCoder.GameInput;

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

        bool hasFocus => (dropMenu as IFocusable).HasFocus();

        bool hasController => controller != null;

        bool isDragging {
            get => controller.isDragging;
            set => controller.isDragging = value;
        }

        Vector2Int previousPosition = Vector2Int.zero;

        Editor editor => InterfaceCanvas.instance.editorVisibiltyManager;

        public void OnPointerDown(PointerEventData eventData) {
            if (!hasController) return;

            if (hasFocus) return;

            HierarchyController.instance.SetOnTopOfNodes(controller);
            controller.SetState(state: "over", propagation: true);
        }

        public void OnBeginDrag(PointerEventData e) {
            if (editor.isEditorOpenRemotely) return;

            if (!hasController) return;

            if (hasFocus) return;
            if (Executer.instance.isRunning && controller.hasParent) return;
            if (controller?.ownTransform.isMovingSmoothly == true) return;

            previousPosition = controller.BeginDrag(e);

            isDragging = true;
        }

        public void OnDrag(PointerEventData e) {
            if (!hasController) return;

            if (hasFocus) return;
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
                controller.SetState(state: "normal", propagation: true);
            }
        }
    }
}