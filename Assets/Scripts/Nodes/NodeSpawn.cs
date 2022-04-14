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
        NodeController spawned;
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
            spawned = Instantiate(nodeController, canvas.transform);

            spawned.gameObject.name = $"{nodeController.gameObject.name} ({spawnedNodes++})";
            spawned.ownTransform.SetPosition(
                x: (int)newPosition.x - (spawned.ownTransform.width * pixelScale) / 2,
                y: (int)newPosition.y + (spawned.ownTransform.initHeight * pixelScale) / 2
            );
            spawned.canvas = canvas;

            spawned.SetMiddleZone(true);

            HierarchyController.instance.SetOnTopOfCanvas(spawned);
            spawned.ownTransform.SetFloatPositionByDelta(dx, dy);

            spawned.isDragging = true;

            spawned.SetState("pressed");
        }

        public void OnDrag(PointerEventData eventData) {
            if (eventData.dragging) {
                spawned.ownTransform.SetFloatPositionByDelta(
                    dx: eventData.delta.x,
                    dy: eventData.delta.y
                );

                spawned.currentDrop = spawned.GetDrop();

                if (spawned.currentDrop != spawned.previousDrop) {
                    spawned.currentDrop?.SetState("over");
                    spawned.previousDrop?.SetState("normal");

                    spawned.previousDrop = spawned.currentDrop;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData) {

            var dragDropZone = spawned.GetDrop();

            if (dragDropZone?.category == "working") {
                if (!spawned.InvokeZones()) HierarchyController.instance.SetOnTopOfNodes(spawned);
                spawned.SetMiddleZone(false);
                spawned.isDragging = false;
                spawned.SetState("over");

                dragDropZone.SetState("normal");
            } else if (dragDropZone?.category == "erasing") {
                spawned.isDragging = false;
                spawned.Disappear();
                spawned.SetState("normal");

                dragDropZone.SetState("normal");
            } else {
                HierarchyController.instance.DeleteNode(spawned);
                Destroy(spawned.gameObject);
            }

            spawned = null;
        }
    }

}