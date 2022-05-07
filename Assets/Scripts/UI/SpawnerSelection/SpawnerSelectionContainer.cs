// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class SpawnerSelectionContainer : MonoBehaviour {

        // Editor variables
        [SerializeField] public ScrollBarController scrollBar;
        [SerializeField] public NodeTransform content;
        [SerializeField] Transform children;

        [SerializeField] ButtonController returnButton;
        [SerializeField] SpawnerSelectionContainerLockButton lockButton;

        [SerializeField] public SpawnerSelectionController categoryController;

        [SerializeField] NodeSpawnController spawnerPrefab;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        public bool isLocked => lockButton.isLocked;

        // State variables
        bool initialized = false;

        List<NodeSpawnController> spawners;

        // Methods
        void Start() {
            // Initialize();
        }

        public void Initialize(List<NodeSpawnTemplate> spawnersTemplates = null) {
            if (initialized) return;

            spawners = CreateSpawners(spawnersTemplates: spawnersTemplates);
            RefreshSpawnerPositions();

            returnButton.AddListener(() => categoryController.LoseFocus());

            SetVisible(visible: false, smooth: false);

            initialized = true;
        }

        List<NodeSpawnController> CreateSpawners(List<NodeSpawnTemplate> spawnersTemplates) {
            return spawnersTemplates?.ConvertAll(t => NodeSpawnController.Create(
                spawnerPrefab: spawnerPrefab,
                parent: children,
                categoryContainer: this,
                template: t
            )) ?? new List<NodeSpawnController>();
        }

        public void AddSpawner(NodeSpawnController spawner, bool smooth = false) {
            spawners.Add(spawner);

            RefreshSpawnerPositions(smooth: smooth);
        }

        void RefreshSpawnerPositions(bool smooth = false) {
            var newY = PositionAllSpawners(smooth: smooth);

            if (newY >= content.initHeight) {
                content.Expand(dy: newY - content.height);
            } else {
                content.Expand(dy: -(content.height - content.initHeight));
            }

            scrollBar.RefreshSlider();
        }

        int PositionAllSpawners(bool smooth = false) {
            var lastY = -14;

            spawners?.ForEach(s => {
                s.ownTransform.SetPosition(x: 18);
                s.ownTransform.SetPosition(x: 18, y: lastY, smooth: smooth);
                lastY -= s.ownTransform.height + 10;
            });

            return -(lastY - 14);
        }

        public void RemoveSpawner(string symbolName, bool smooth = false) {
            var spawner = spawners.Find(s => s.symbolName == symbolName);

            SymbolTable.instance.DeleteSymbol(symbolName);
            spawners.Remove(spawner);

            spawner.ownTransform.SetPositionByDelta(
                dx: -1000,
                smooth: true,
                endingCallback: () => Destroy(spawner.gameObject)
            );

            RefreshSpawnerPositions(smooth: smooth);
        }

        public void SetVisible(bool visible, bool smooth = false) {
            ownTransform.SetPosition(
                x: visible ? 16 : -(ownTransform.width + 10),
                smooth: smooth,
                endingCallback: visible ? () => categoryController.GetFocus() : (System.Action)null
            );
        }

    }
}