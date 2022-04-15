// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


using ScrapCoder.Interpreter;

namespace ScrapCoder.VisualNodes {

    public class NodeSpawn : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

        // Editor variables
        [SerializeField] Canvas canvas;
        [SerializeField] NodeType nodeToSpawn;
        [SerializeField] int limit;

        // State variables
        string _symbolName = null;
        public string symbolName {
            get => _symbolName ??= nodeToSpawn.ToString();
            set => _symbolName = value;
        }

        // Lazy and other variables
        NodeController spawned;
        const int pixelScale = 2;

        RectTransform _canvasTransform;
        RectTransform canvasTransform => _canvasTransform ??= canvas.GetComponent<RectTransform>();

        // Methods
        public void OnBeginDrag(PointerEventData e) {
            var newPosition = GetPointerPosition(e);
            Spawn(newPosition, e);
        }

        public void OnDrag(PointerEventData e) {
            if (!e.dragging || spawned == null) return;

            spawned.ownTransform.SetFloatPositionByDelta(dx: e.delta.x, dy: e.delta.y);

            spawned.currentDrop = spawned.GetDrop();

            if (spawned.currentDrop != spawned.previousDrop) {
                spawned.currentDrop?.SetState("over");
                spawned.previousDrop?.SetState("normal");

                spawned.previousDrop = spawned.currentDrop;
            }
        }

        public void OnEndDrag(PointerEventData e) {

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
                SymbolTable.instance[symbolName].Remove(spawned);
                HierarchyController.instance.DeleteNode(spawned);
                Destroy(spawned.gameObject);
            }

            spawned = null;
        }

        Vector2 GetPointerPosition(PointerEventData eventData) {
            var newPosition = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect: canvasTransform,
                screenPoint: eventData.position,
                cam: canvas.worldCamera,
                localPoint: out newPosition
            );

            return newPosition;
        }

        void Spawn(Vector2 newPosition, PointerEventData e) {

            if (SymbolTable.instance[symbolName] == null) {
                SymbolTable.instance.AddSymbol(
                    limit: limit,
                    symbolName: symbolName,
                    type: nodeToSpawn
                );
            } else if (SymbolTable.instance[symbolName].isFull) {
                return;
            }

            spawned = Instantiate(
                original: NodeDictionaryController.instance[nodeToSpawn],
                parent: canvas.transform
            );

            SymbolTable.instance[symbolName].Add(spawned);

            spawned.symbolName = symbolName;
            spawned.gameObject.name = $"{symbolName}[{SymbolTable.instance[symbolName].Count}]";

            spawned.ownTransform.SetPosition(
                x: (int)newPosition.x - (spawned.ownTransform.width * pixelScale) / 2,
                y: (int)newPosition.y + (spawned.ownTransform.initHeight * pixelScale) / 2
            );

            spawned.workingZone = HierarchyController.instance.workingZone;

            spawned.SetMiddleZone(true);

            HierarchyController.instance.SetOnTopOfCanvas(spawned);
            spawned.ownTransform.SetFloatPositionByDelta(dx: e.delta.x, dy: e.delta.y);

            spawned.isDragging = true;

            spawned.SetState("over");
        }
    }

}