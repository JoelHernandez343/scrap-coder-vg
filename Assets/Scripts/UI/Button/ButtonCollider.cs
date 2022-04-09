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
        [SerializeField] ButtonVisualState normalState;
        [SerializeField] ButtonVisualState overState;
        [SerializeField] ButtonVisualState pressedState;
        [SerializeField] ButtonVisualState deactivatedState;

        [SerializeField] ButtonController buttonController;

        // Lazy variables
        bool usingSimpleSprites => buttonController.usingSimpleSprites;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool over;

        // Methods
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            SetStateVisible("pressed");
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            SetStateVisible(over ? "over" : "normal");
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            over = true;
            SetStateVisible("over");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            over = false;
            SetStateVisible("normal");
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            buttonController.OnClick();
        }

        public void SetActive(bool active) {
            SetStateVisible(active ? "normal" : "deactivated");
            gameObject.SetActive(active);
        }

        public void SetStateVisible(string state) {
            normalState.SetVisible(state == "normal", usingSimpleSprites ? "sprite" : "shape");
            overState.SetVisible(state == "over", usingSimpleSprites ? "sprite" : "shape");
            pressedState.SetVisible(state == "pressed", usingSimpleSprites ? "sprite" : "shape");
            deactivatedState.SetVisible(state == "deactivated", usingSimpleSprites ? "sprite" : "shape");
        }
    }
}