/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeSpawner : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    int spawnedNodes = 0;
    Vector3 dragOffset;
    NodeController3 spawnedNode;

    [SerializeField] Canvas canvasToSpawn;
    [SerializeField] NodeController3 nodeToSpawn;



    public void OnBeginDrag(PointerEventData eventData) {
        dragOffset = transform.position - Utils.GetMousePosition();

        spawnedNode = Instantiate(nodeToSpawn, canvasToSpawn.transform);

        spawnedNode.container.name = $"{nodeToSpawn.gameObject.name} ({spawnedNodes++})";
        spawnedNode.container.transform.SetAsLastSibling();
        spawnedNode.container.transform.position = Utils.GetMousePosition() + dragOffset;
        spawnedNode.ToggleGreenTrigger(true);
        spawnedNode.DetachFromParent();
    }

    public void OnDrag(PointerEventData eventData) {
        spawnedNode.container.transform.position = Utils.GetMousePosition() + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData) {
        spawnedNode.InvokeTriggers();
        spawnedNode.ToggleGreenTrigger(false);

        spawnedNode = null;
    }

    public void OnPointerDown(PointerEventData eventData) {

    }
}
