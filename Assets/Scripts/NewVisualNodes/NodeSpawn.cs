using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeSpawn : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    int spawnedNodes = 0;
    NodeController spawnedNode;

    [SerializeField] Canvas canvas;
    [SerializeField] NodeController nodeToSpawn;
    [SerializeField] NodeTransform nodeTransform;

    public void OnBeginDrag(PointerEventData eventData) {
        var (dx, dy) = (eventData.delta.x, eventData.delta.y);

        spawnedNode = Instantiate(nodeToSpawn, canvas.transform);

        spawnedNode.gameObject.name = $"{nodeToSpawn.gameObject.name} (${spawnedNodes++})";
        spawnedNode.nodeTransform.SetPosition(nodeTransform.position);
        spawnedNode.canvas = canvas;

        spawnedNode.SetMiddleZone(true);
        spawnedNode.DetachFromParent();

        spawnedNode.transform.SetAsLastSibling();
        HierarchyController.instance.SetOnTop(spawnedNode);
        spawnedNode.nodeTransform.SetFloatPositionByDelta(dx, dy);
    }

    public void OnDrag(PointerEventData eventData) {
        var (dx, dy) = (eventData.delta.x, eventData.delta.y);

        spawnedNode.nodeTransform.SetFloatPositionByDelta(dx, dy);
    }

    public void OnEndDrag(PointerEventData eventData) {
        spawnedNode.InvokeZones();
        spawnedNode.SetMiddleZone(false);

        spawnedNode = null;
    }
}
