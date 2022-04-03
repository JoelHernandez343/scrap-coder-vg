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
        // Shapes
        [SerializeField] NodeShape normalState;
        [SerializeField] NodeShape overState;
        [SerializeField] NodeShape pressedState;
        [SerializeField] NodeShape deactivatedState;

        // Sprites
        [SerializeField] NodeSprite normalStateSprite;
        [SerializeField] NodeSprite overStateSprite;
        [SerializeField] NodeSprite pressedStateSprite;
        [SerializeField] NodeSprite deactivatedStateSprite;

        [SerializeField] ButtonController buttonController;

        // Lazy variables
        bool usingSimpleSprites => buttonController.usingSimpleSprites;

        List<NodeTransform> _transformShapes;
        public List<NodeTransform> transformShapes => _transformShapes ??= new List<NodeTransform> {
            normalState.ownTransform,
            overState.ownTransform,
            pressedState.ownTransform,
            deactivatedState.ownTransform
        };

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

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
            SetSpriteStateVisible(usingSimpleSprites ? state : "nothing");
            SetShapeStateVisible(usingSimpleSprites ? "nothing" : state);
        }

        void SetShapeStateVisible(string state) {
            normalState.SetVisible(state == "normal");
            overState.SetVisible(state == "over");
            pressedState.SetVisible(state == "pressed");
            deactivatedState.SetVisible(state == "deactivated");
        }

        void SetSpriteStateVisible(string state) {
            normalStateSprite.SetVisible(state == "normal");
            overStateSprite.SetVisible(state == "over");
            pressedStateSprite.SetVisible(state == "pressed");
            deactivatedStateSprite.SetVisible(state == "deactivated");
        }
    }
}