// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {

    public class NodeSpawn : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

        // Editor variables
        [SerializeField] Canvas canvas;
        [SerializeField] NodeType nodeToSpawn;

        // State variables
        int spawnedNodes = 0;

        // Lazy and other variables
        NodeController spawnedNode;
        const int pixelScale = 2;

        RectTransform _canvasTransform;
        RectTransform canvasTransform => _canvasTransform ??= canvas.GetComponent<RectTransform>();

        // Methods
        public void OnBeginDrag(PointerEventData eventData) {
            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            var newPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: canvasTransform,
                screenPoint: eventData.position,
                cam: canvas.worldCamera,
                localPoint: out newPosition
            );

            var nodeController = NodeDictionaryController.instance[nodeToSpawn];
            spawnedNode = Instantiate(nodeController, canvas.transform);

            spawnedNode.gameObject.name = $"{nodeController.gameObject.name} ({spawnedNodes++})";
            spawnedNode.ownTransform.SetPosition(
                x: (int)newPosition.x - (spawnedNode.ownTransform.width * pixelScale) / 2,
                y: (int)newPosition.y + (spawnedNode.ownTransform.initHeight * pixelScale) / 2
            );
            spawnedNode.canvas = canvas;

            spawnedNode.SetMiddleZone(true);
            spawnedNode.DetachFromParent();

            HierarchyController.instance.SetOnTop(spawnedNode);
            spawnedNode.ownTransform.SetFloatPositionByDelta(dx, dy);

            spawnedNode.isDragging = true;
        }

        public void OnDrag(PointerEventData eventData) {
            if (eventData.dragging) {
                spawnedNode.ownTransform.SetFloatPositionByDelta(
                    dx: eventData.delta.x,
                    dy: eventData.delta.y
                );

                spawnedNode.currentDragDropZone = spawnedNode.GetDragDropZone();

                if (spawnedNode.currentDragDropZone != spawnedNode.previousDragDropZone) {
                    spawnedNode.currentDragDropZone?.SetState("over");
                    spawnedNode.previousDragDropZone?.SetState("normal");

                    spawnedNode.previousDragDropZone = spawnedNode.currentDragDropZone;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData) {

            var dragDropZone = spawnedNode.GetDragDropZone();

            if (dragDropZone?.category == "working") {
                spawnedNode.InvokeZones();
                spawnedNode.SetMiddleZone(false);
                spawnedNode.isDragging = false;

                dragDropZone.SetState("normal");
            } else if (dragDropZone?.category == "erasing") {
                spawnedNode.isDragging = false;
                spawnedNode.Disappear();

                dragDropZone.SetState("normal");
            } else {
                HierarchyController.instance.Delete(spawnedNode);
                Destroy(spawnedNode.gameObject);
            }

            spawnedNode = null;
        }
    }

}