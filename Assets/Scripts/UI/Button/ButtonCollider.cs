// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class ButtonCollider :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler {

        // Editor variables
        [SerializeField] ButtonController controller;
        [SerializeField] List<NodeTransform> itemsToDown;
        [SerializeField] bool canDrag = false;

        // State variables
        bool over;
        bool isPressed = false;
        bool isDragging = false;

        // Methods
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            controller.SetState("pressed");
            itemsToDown.ForEach(i => i.SetPositionByDelta(dy: -1));

            isPressed = true;

            if (canDrag) {
                isDragging = true;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if (canDrag) {
                isDragging = false;
            }

            isPressed = false;

            controller.SetState(over ? "over" : "normal");
            itemsToDown.ForEach(i => i.ResetYToRelative());
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (controller.ownTransform.isMovingSmoothly) return;

            over = true;

            controller.SetState(isDragging ? "pressed" : "over");
            controller.OnTriggerEvent(eventType: ButtonEventType.OnPointerEnter);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            over = false;

            if (!isDragging) {
                controller.SetState("normal");
                controller.OnTriggerEvent(eventType: ButtonEventType.OnPointerExit);
            }

            if (isPressed) {
                itemsToDown.ForEach(i => i.ResetYToRelative());
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            controller.OnTriggerEvent(eventType: ButtonEventType.OnClick);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            if (canDrag) {
                isDragging = true;
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (canDrag) {
                isDragging = true;
                controller.SetState("pressed");
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            if (canDrag) {
                isDragging = false;
                controller.SetState(over ? "over" : "normal");
            }
        }

    }
}