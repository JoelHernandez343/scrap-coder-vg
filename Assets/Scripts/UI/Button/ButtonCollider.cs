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
        IPointerClickHandler {

        // Editor variables
        [SerializeField] ButtonController controller;

        // State variables
        bool over;

        // Methods
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            controller.SetState("pressed");
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            controller.SetState(over ? "over" : "normal");
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            over = true;
            controller.SetState("over");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            over = false;
            controller.SetState("normal");
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            controller.OnClick();
        }
    }
}