// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.Interpreter;
using ScrapCoder.UI;
using ScrapCoder.Audio;

namespace ScrapCoder.VisualNodes {

    public class NodeSpawnController : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeType nodeToSpawn;
        [SerializeField] int spawnLimit;

        [SerializeField] public string title;
        [SerializeField] ExpandableText titleText;

        [SerializeField] NodeShapeContainer shapeState;

        [SerializeField] NodeSprite iconSprite;
        [SerializeField] NodeSprite infinityIcon;
        [SerializeField] ExpandableText counter;

        [SerializeField] ButtonController discardButton;

        // State variables
        string _symbolName = null;
        public string symbolName {
            get => _symbolName ??= nodeToSpawn.ToString();
            set => _symbolName = value;
        }

        NodeController _prefabToSpawn;
        public NodeController prefabToSpawn {
            get => _prefabToSpawn ??= NodeDictionaryController.instance[nodeToSpawn];
            set => _prefabToSpawn = value;
        }

        string state;
        string selectedIcon;

        bool initialized = false;

        public SpawnerSelectionContainer categoryContainer;

        // Lazy and other variables
        [System.NonSerialized] public NodeController spawned;
        const int pixelScale = 2;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        SoundScript _sound;
        SoundScript sound => _sound ??= GetComponent<SoundScript>() as SoundScript;

        int spawnedCount => SymbolTable.instance[symbolName]?.Count ?? 0;
        bool showingInfinity => spawnLimit == -1;

        // Methods
        void Start() {
            Initialize();
        }

        void Initialize() {
            if (initialized) return;

            SetIcon();
            SetTitle();
            SetState("normal");
            SetDiscardButton();
            RefreshCounter();
            SetSymbol();

            initialized = true;
        }

        void SetTitle() {
            if (titleText.text != "") return;

            titleText.ChangeText(
                newText: title,
                minWidth: 0,
                lettersOffset: 9
            );
        }

        void SetDiscardButton(System.Action discardCallback = null) {
            discardCallback ??= () => {
                if (Executer.instance.isRunning) {
                    MessagesController.instance.AddMessage(
                        message: $"No puedes borrar el nodo: {symbolName} mientras el ejecutor trabaja.",
                        type: MessageType.Warning
                    );
                    return;
                }

                SymbolTable.instance[symbolName]?.DeleteAllNodes();
            };

            if (discardButton?.GetListenersCount(ButtonEventType.OnClick) == 0) {
                discardButton.AddListener(discardCallback);
            }
        }

        void SetIcon() {
            iconSprite.SetState(selectedIcon);

            iconSprite.ownTransform.SetPosition(
                y: -(ownTransform.height / 2) + iconSprite.ownTransform.height / 2
            );
        }

        void SetSymbol() {
            if (SymbolTable.instance[symbolName] != null) return;

            SymbolTable.instance.AddSymbol(
                limit: spawnLimit,
                symbolName: symbolName,
                type: nodeToSpawn,
                spawner: this
            );
        }

        public bool SpawnNode(Vector2 newPosition, float dx, float dy) {

            if (!InstantiateNode()) return false;

            HierarchyController.instance.SetOnTopOfEditor(spawned);

            spawned.ownTransform.SetPosition(
                x: (int)newPosition.x - (spawned.ownTransform.width * pixelScale) / 2,
                y: (int)newPosition.y + (spawned.ownTransform.initHeight * pixelScale) / 2
            );
            spawned.ownTransform.SetFloatPositionByDelta(dx: dx, dy: dy);
            spawned.ownTransform.SetScale(x: InterfaceCanvas.NodeScaleFactor, y: InterfaceCanvas.NodeScaleFactor, z: 1);

            spawned.symbolName = symbolName;
            spawned.gameObject.name = $"{symbolName}[{SymbolTable.instance[symbolName].Count}]";
            spawned.isDragging = true;

            spawned.SetMiddleZone(true);
            spawned.SetState(state: "over", propagation: true);

            sound.PlayClip();

            return true;
        }

        bool InstantiateNode() {
            if (SymbolTable.instance[symbolName] == null) {
                throw new System.InvalidOperationException($"This symbol has been deleted: {symbolName}");
            } else if (SymbolTable.instance[symbolName].isFull) {
                return false;
            }

            spawned = NodeController.Create(
                prefab: prefabToSpawn,
                parent: InterfaceCanvas.instance.editor.transform,
                template: new NodeControllerTemplate {
                    symbolName = symbolName,
                    name = $"{symbolName}[{SymbolTable.instance[symbolName].Count}]"
                }
            );

            return true;
        }

        public void RemoveSpawned() {
            spawned.DeleteSelf(deleteChildren: false);

            ClearSpawned();
        }

        public void ClearSpawned() => spawned = null;

        public void SetState(string state) {
            if (this.state == state) return;

            this.state = state;

            shapeState?.SetState(state);
        }

        public void RefreshCounter() {
            if (showingInfinity) return;

            infinityIcon?.SetVisible(false);
            counter?.ChangeText($"{spawnLimit - spawnedCount}", 0, 0);
        }

        public void HideContainer() {
            if (!categoryContainer.isLocked) {
                categoryContainer.categoryController.LoseFocus();
            }
        }

        public static NodeSpawnController Create(
            NodeSpawnController spawnerPrefab,
            Transform parent,
            NodeSpawnTemplate template,
            SpawnerSelectionContainer categoryContainer,
            NodeController prefabToSpawn = null,
            System.Action discardCallback = null
        ) {
            var newSpawner = Instantiate(original: spawnerPrefab, parent: parent);

            newSpawner.ownTransform.depth = 0;
            newSpawner.ownTransform.SetScale(x: 1, y: 1, z: 1);

            newSpawner.name = $"spawner_{template.symbolName}";
            newSpawner.symbolName = template.symbolName;
            newSpawner.spawnLimit = template.spawnLimit;
            newSpawner.title = template.title;
            newSpawner.selectedIcon = template.selectedIcon;

            newSpawner.nodeToSpawn = template.nodeToSpawn;
            newSpawner.prefabToSpawn = prefabToSpawn;

            newSpawner.categoryContainer = categoryContainer;

            newSpawner.SetDiscardButton(discardCallback: discardCallback);

            newSpawner.Initialize();

            return newSpawner;
        }
    }

}