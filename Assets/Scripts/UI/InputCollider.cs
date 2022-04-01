// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.UI {
    public class InputCollider : MonoBehaviour, IPointerDownHandler {
        // Editor variables
        [SerializeField] Camera mainCamera;
        [SerializeField] InputText inputText;

        // Lazy variables
        VisualNodes.NodeTransform _ownTransform;
        VisualNodes.NodeTransform ownTransform
            => _ownTransform ??= GetComponent<VisualNodes.NodeTransform>();

        // Methods
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            var clickPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: ownTransform.rectTransform,
                    screenPoint: eventData.position,
                    cam: mainCamera,
                    localPoint: out clickPosition
                );

            inputText.Click(clickPosition.x);
        }
    }
}