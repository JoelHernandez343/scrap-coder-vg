// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ScrapCoder.Interpreter;
using ScrapCoder.UI;
using ScrapCoder.Tutorial;

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
        Editor editor => InterfaceCanvas.instance.editorVisibiltyManager;

        public void OnBeginDrag(PointerEventData e) {
            if (editor.isEditorOpenRemotely) return;
            if (Executer.instance.isRunning) return;

            spawnController.SetState("normal");

            var couldBeSpawned = spawnController.SpawnNode(
                newPosition: GetPointerPosition(e),
                dx: e.delta.x,
                dy: e.delta.y
            );

            if (couldBeSpawned) {
                spawnController.HideContainer();
            }
        }

        public void OnDrag(PointerEventData e) {
            if (!e.dragging || spawned == null) return;

            spawned.OnDrag(e);
        }

        public void OnEndDrag(PointerEventData e) {
            if (spawned == null) return;

            if (spawned.OnEndDrag(discardCallback: () => spawnController.RemoveSpawned())) {
                TutorialController.instance.ReceiveSignal(signal: $"placedType{spawned.type}");
            }
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
                rect: InterfaceCanvas.instance.editor.rectTransform,
                screenPoint: eventData.position,
                cam: InterfaceCanvas.instance.currentCamera,
                localPoint: out newPosition
            );

            return newPosition;
        }

    }
}