// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrapCoder.VisualNodes {
    public class NodeSpawnCollider : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

        [SerializeField] NodeSpawnController spawnController;

        NodeController spawned => spawnController.spawned;

        void IBeginDragHandler.OnBeginDrag(PointerEventData e) {
            spawnController.SpawnNode(
                newPosition: GetPointerPosition(e),
                dx: e.delta.x,
                dy: e.delta.y
            );
        }

        void IDragHandler.OnDrag(PointerEventData e) {
            if (!e.dragging || spawned == null) return;

            spawned.ownTransform.SetFloatPositionByDelta(dx: e.delta.x, dy: e.delta.y);

            spawned.currentDrop = spawned.GetDrop();

            if (spawned.currentDrop != spawned.previousDrop) {
                spawned.currentDrop?.SetState("over");
                spawned.previousDrop?.SetState("normal");

                spawned.previousDrop = spawned.currentDrop;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData e) {
            if (spawned == null) return;

            var dragDropZone = spawned.GetDrop();

            if (dragDropZone?.category == "working") {
                if (!spawned.InvokeZones()) HierarchyController.instance.SetOnTopOfNodes(spawned);
                spawned.SetMiddleZone(false);
                spawned.isDragging = false;
                spawned.SetState("normal");

                dragDropZone.SetState("normal");
            } else if (dragDropZone?.category == "erasing") {
                spawned.isDragging = false;
                spawned.Disappear();
                spawned.SetState("normal");

                dragDropZone.SetState("normal");
            } else {
                spawnController.RemoveSpawned();
            }

            spawnController.ClearSpawned();
        }

        Vector2 GetPointerPosition(PointerEventData eventData) {
            var newPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: spawnController.canvasTransform,
                screenPoint: eventData.position,
                cam: spawnController.canvas.worldCamera,
                localPoint: out newPosition
            );

            return newPosition;
        }


    }
}