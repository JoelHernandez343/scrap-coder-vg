// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class InputTextConlliderForNodes :
        MonoBehaviour,
        IPointerDownHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerUpHandler,
        IPointerClickHandler {

        // Editor variables
        [SerializeField] InputText inputText;

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform
            => _ownTransform ??= GetComponent<NodeTransform>() as NodeTransform;

        Camera _mainCamera;
        Camera mainCamera =>
            _mainCamera ??= (GameObject.FindGameObjectWithTag("MainCamera") as GameObject)?.GetComponent<Camera>() as Camera;

        NodeController controller => ownTransform.controller;

        bool isDragging {
            get => controller.isDragging;
            set => controller.isDragging = value;
        }

        bool hasFocus => (inputText as InputManagment.IFocusable).HasFocus();

        bool wasDragging;

        Vector2Int previousPosition = Vector2Int.zero;

        public void OnPointerClick(PointerEventData e) {

            if (!wasDragging && !Executer.instance.isRunning) {
                GetFocus(e);
            }

            wasDragging = false;
        }

        public void OnPointerDown(PointerEventData e) {
            HierarchyController.instance.SetOnTopOfNodes(controller);
            controller.SetState("over");
        }

        public void OnBeginDrag(PointerEventData e) {
            if (Executer.instance.isRunning) return;
            if (ownTransform.isMovingSmoothly) return;

            previousPosition = controller.BeginDrag(e);

            isDragging = true;
            wasDragging = true;
        }

        public void OnDrag(PointerEventData e) {
            if (Executer.instance.isRunning) return;
            if (controller.ownTransform.isMovingSmoothly) return;

            controller.OnDrag(e);
        }

        public void OnEndDrag(PointerEventData e) {
            if (isDragging) {
                controller.OnEndDrag(previousPosition);
            }
        }

        public void OnPointerUp(PointerEventData e) {
            if (!isDragging && !hasFocus) {
                controller.SetState("normal");
            }
        }

        void GetFocus(PointerEventData e) {
            controller.SetState("over");

            if (!hasFocus) {
                InputManagment.InputController.instance.SetFocusOn(inputText);
            }

            var clickPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: ownTransform.rectTransform,
                screenPoint: e.position,
                cam: mainCamera,
                localPoint: out clickPosition
            );

            inputText.Click(clickPosition.x);
        }
    }
}