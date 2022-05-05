// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.Interpreter;
using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {
    public class NodeSpawnCollider :
        MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler {

        [SerializeField] NodeSpawnController spawnController;

        NodeController spawned => spawnController.spawned;

        public void OnBeginDrag(PointerEventData e) {
            if (Executer.instance.isRunning) return;

            spawnController.SetState("normal");
            spawnController.HideContainer();
            spawnController.SpawnNode(
                newPosition: GetPointerPosition(e),
                dx: e.delta.x,
                dy: e.delta.y
            );
        }

        public void OnDrag(PointerEventData e) {
            if (!e.dragging || spawned == null) return;

            spawned.OnDrag(e);
        }

        public void OnEndDrag(PointerEventData e) {
            if (spawned == null) return;

            spawned.OnEndDrag(discardCallback: () => spawnController.RemoveSpawned());
            spawnController.ClearSpawned();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            spawnController.SetState("over");
        }

        public void OnPointerExit(PointerEventData eventData) {
            spawnController.SetState("normal");
        }

        Vector2 GetPointerPosition(PointerEventData eventData) {
            var newPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: InterfaceCanvas.instance.nodeEditorContainer.rectTransform,
                screenPoint: eventData.position,
                cam: InterfaceCanvas.instance.camera,
                localPoint: out newPosition
            );

            return newPosition;
        }

    }
}