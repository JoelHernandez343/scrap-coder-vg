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
        [SerializeField] NodeController nodeToSpawn;
        [SerializeField] NodeTransform ownTransform;

        // State variables
        int spawnedNodes = 0;

        // Lazy and other variables
        NodeController spawnedNode;

        // Methods
        public void OnBeginDrag(PointerEventData eventData) {
            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            spawnedNode = Instantiate(nodeToSpawn, canvas.transform);

            spawnedNode.gameObject.name = $"{nodeToSpawn.gameObject.name} ({spawnedNodes++})";
            spawnedNode.ownTransform.SetPosition(ownTransform.position);
            spawnedNode.canvas = canvas;

            spawnedNode.SetMiddleZone(true);
            spawnedNode.DetachFromParent();

            HierarchyController.instance.SetOnTop(spawnedNode);
            spawnedNode.ownTransform.SetFloatPositionByDelta(dx, dy);
        }

        public void OnDrag(PointerEventData eventData) {
            var (dx, dy) = (eventData.delta.x, eventData.delta.y);

            spawnedNode.ownTransform.SetFloatPositionByDelta(dx, dy);
        }

        public void OnEndDrag(PointerEventData eventData) {
            spawnedNode.InvokeZones();
            spawnedNode.SetMiddleZone(false);

            spawnedNode = null;
        }
    }

}