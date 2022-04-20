// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.Interpreter;
using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {

    public class NodeSpawnController : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] public Canvas canvas;
        [SerializeField] NodeType nodeToSpawn;
        [SerializeField] public int limit;

        [SerializeField] public string text;
        [SerializeField] ExpandableText expandableText;

        [SerializeField] List<NodeTransform> itemsToExpand;

        // State variables
        string _symbolName = null;
        public string symbolName {
            get => _symbolName ??= nodeToSpawn.ToString();
            set => _symbolName = value;
        }

        NodeController _prefab;
        public NodeController prefab {
            get => _prefab ??= NodeDictionaryController.instance[nodeToSpawn];
            set => _prefab = value;
        }

        // Lazy and other variables
        [System.NonSerialized] public NodeController spawned;
        const int pixelScale = 2;

        RectTransform _canvasTransform;
        public RectTransform canvasTransform => _canvasTransform ??= canvas.GetComponent<RectTransform>();

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        void Start() {
            ExpandByText();
        }

        void ExpandByText() {
            if (expandableText.text != "") return;

            var dx = expandableText.ChangeText(
                newText: text,
                minWidth: 0,
                lettersOffset: 9
            );

            ownTransform.Expand(dx: dx, smooth: false);
        }

        public void SpawnNode(Vector2 newPosition, float dx, float dy) {

            if (!InstantiateNode()) return;

            HierarchyController.instance.SetOnTopOfCanvas(spawned);

            spawned.ownTransform.SetPosition(
                x: (int)newPosition.x - (spawned.ownTransform.width * pixelScale) / 2,
                y: (int)newPosition.y + (spawned.ownTransform.initHeight * pixelScale) / 2
            );
            spawned.ownTransform.SetFloatPositionByDelta(dx: dx, dy: dy);
            spawned.ownTransform.SetScale(x: 2, y: 2, z: 1);

            spawned.symbolName = symbolName;
            spawned.gameObject.name = $"{symbolName}[{SymbolTable.instance[symbolName].Count}]";
            spawned.workingZone = HierarchyController.instance.workingZone;
            spawned.isDragging = true;

            spawned.SetMiddleZone(true);
            spawned.SetState("over");
        }

        bool InstantiateNode() {
            if (SymbolTable.instance[symbolName] == null) {
                SymbolTable.instance.AddSymbol(
                    limit: limit,
                    symbolName: symbolName,
                    type: nodeToSpawn
                );
            } else if (SymbolTable.instance[symbolName].isFull) {
                return false;
            }

            spawned = Instantiate(original: prefab, parent: canvas.transform);

            SymbolTable.instance[symbolName].Add(spawned);

            return true;
        }

        public void RemoveSpawned() {
            SymbolTable.instance[symbolName].Remove(spawned);
            Destroy(spawned.gameObject);

            ClearSpawned();
        }

        public void ClearSpawned() {
            spawned = null;
        }

        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded expanded) {
            itemsToExpand.ForEach(i => i.Expand(dx: dx, dy: dy, smooth: smooth));

            return (dx, dy);
        }
    }

}