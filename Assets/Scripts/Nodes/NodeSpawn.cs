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

        // Methods
        public void OnBeginDrag(PointerEventData eventData) {
            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            var newPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                canvas.worldCamera,
                out newPosition
            );

            var nodeController = NodeDictionaryController.instance[nodeToSpawn];
            spawnedNode = Instantiate(nodeController, canvas.transform);

            spawnedNode.gameObject.name = $"{nodeController.gameObject.name} ({spawnedNodes++})";
            spawnedNode.ownTransform.SetPosition((
                (int)newPosition.x - (spawnedNode.ownTransform.width * pixelScale) / 2,
                (int)newPosition.y + (spawnedNode.ownTransform.initHeight * pixelScale) / 2
            ));
            spawnedNode.canvas = canvas;

            spawnedNode.SetMiddleZone(true);
            spawnedNode.DetachFromParent();

            HierarchyController.instance.SetOnTop(spawnedNode);
            spawnedNode.ownTransform.SetFloatPositionByDelta(dx, dy);
        }

        public void OnDrag(PointerEventData eventData) {
            if (eventData.dragging) {
                var (dx, dy) = (eventData.delta.x, eventData.delta.y);

                spawnedNode.ownTransform.SetFloatPositionByDelta(dx, dy);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            spawnedNode.InvokeZones();
            spawnedNode.SetMiddleZone(false);

            spawnedNode = null;
        }
    }

}