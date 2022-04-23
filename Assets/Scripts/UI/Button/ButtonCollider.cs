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
        [SerializeField] bool canDrag = false;

        // State variables
        bool over;
        bool isDragging = false;

        // Methods
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            controller.SetState("pressed");

            if (canDrag) {
                isDragging = true;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if (canDrag) {
                isDragging = false;
            }

            controller.SetState(over ? "over" : "normal");
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            over = true;

            controller.SetState(isDragging ? "pressed" : "over");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            over = false;

            if (!isDragging) {
                controller.SetState("normal");
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {

            controller.OnClick();
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