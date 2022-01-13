/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeDragger : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    Vector3 dragOffset;
    GameObject container;
    CanvasGroup canvasGroup;

    NodeController3 _controller;
    public NodeController3 controller {
        set {
            _controller = value;
            container = value.container;
        }
        get => _controller;
    }

    public void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        container?.transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;

        if (container != null) {
            dragOffset = container.transform.position - Utils.GetMousePosition();

            controller.ToggleGreenTrigger(true);
            controller.DetachFromParent();
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (container != null) {
            container.transform.position = Utils.GetMousePosition() + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        controller?.InvokeTriggers();
        controller?.ToggleGreenTrigger(false);
    }
}
