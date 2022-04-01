// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.UI {
    public class InputFocusRemover : MonoBehaviour, IPointerDownHandler {

        // Editor variables
        [SerializeField] Canvas canvas;

        // Lazy and other variables
        VisualNodes.INodeExpander _expander;
        VisualNodes.INodeExpander expander
            => _expander ??= (GetComponent<VisualNodes.ColliderExpander>() as VisualNodes.INodeExpander);

        Vector2? _canvasDimensions;
        Vector2 canvasDimensions => _canvasDimensions ??= canvas.GetComponent<RectTransform>().rect.size;

        void Awake() {
            expander.Expand(
                dx: (int)System.Math.Round(canvasDimensions.x),
                dy: (int)System.Math.Round(canvasDimensions.y)
            );
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            InputManagment.InputController.instance.handlerWithFocus.LoseFocus();
            InputManagment.InputController.instance.ClearFocus();
        }
    }
}