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
        [SerializeField] NodeShape normalState;
        [SerializeField] NodeShape overState;
        [SerializeField] NodeShape pressedState;
        [SerializeField] NodeShape deactivatedState;

        [SerializeField] ButtonController buttonController;

        // Methods
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            SetStateVisible("pressed");
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            SetStateVisible("normal");
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            SetStateVisible("over");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            SetStateVisible("normal");
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            buttonController.OnClick();
        }

        public void SetActive(bool active) {
            SetStateVisible(active ? "normal" : "deactivated");
            gameObject.SetActive(active);
        }

        void SetStateVisible(string state) {
            normalState.SetVisible(state == "normal");
            overState.SetVisible(state == "over");
            pressedState.SetVisible(state == "pressed");
            deactivatedState.SetVisible(state == "deactivated");
        }
    }
}