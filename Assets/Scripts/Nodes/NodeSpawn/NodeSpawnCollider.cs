// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.Interpreter;

namespace ScrapCoder.VisualNodes {
    public class NodeSpawnCollider : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

        [SerializeField] NodeSpawnController spawnController;

        NodeController spawned => spawnController.spawned;

        void IBeginDragHandler.OnBeginDrag(PointerEventData e) {
            if (Executer.instance.isRunning) return;

            spawnController.SpawnNode(
                newPosition: GetPointerPosition(e),
                dx: e.delta.x,
                dy: e.delta.y
            );
        }

        void IDragHandler.OnDrag(PointerEventData e) {
            if (!e.dragging || spawned == null) return;

            spawned.OnDrag(e);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData e) {
            if (spawned == null) return;

            spawned.OnEndDrag(discardCallback: () => spawnController.RemoveSpawned());

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