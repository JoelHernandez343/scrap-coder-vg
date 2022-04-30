// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class InputTextCollider : MonoBehaviour, IPointerDownHandler {
        // Editor variables
        [SerializeField] InputText inputText;

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform
            => _ownTransform ??= GetComponent<NodeTransform>() as NodeTransform;

        // Methods
        public void OnPointerDown(PointerEventData eventData) {
            if (!(inputText as InputManagment.IFocusable).HasFocus()) {
                InputManagment.InputController.instance.SetFocusOn(inputText);
            }

            var clickPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: ownTransform.rectTransform,
                screenPoint: eventData.position,
                cam: InterfaceCanvas.instance.camera,
                localPoint: out clickPosition
            );

            inputText.Click(clickPosition.x);
        }
    }
}