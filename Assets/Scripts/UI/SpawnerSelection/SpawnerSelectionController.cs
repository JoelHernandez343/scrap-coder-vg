// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SpawnerSelectionController : MonoBehaviour {

        // Editor variables
        [SerializeField] SpawnerSelectionButton button;
        [SerializeField] SpawnerSelectionContainer container;

        // State variables
        bool initialized = false;

        SelectionController selectionController;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        void Initialize(List<NodeSpawnTemplate> spawnersTemplates, string title, string icon, SelectionController selectionController) {
            if (initialized) return;

            this.selectionController = selectionController;

            ConfigureContainer(spawnersTemplates);
            ConfigureButton(title: title, icon: icon);

            initialized = true;
        }

        void ConfigureContainer(List<NodeSpawnTemplate> spawnersTemplates) {
            container.Initialize(spawnersTemplates: spawnersTemplates);
        }

        void ConfigureButton(string title, string icon) {
            button.Initialize(title: title, icon: icon);
        }

        public void GetFocus() {
            container.SetVisible(visible: true, smooth: true);
            selectionController.HideAllButtons();
        }

        public void LoseFocus() {
            container.SetVisible(visible: false, smooth: true);
            selectionController.ShowAllButtons();
        }

        public void SetButtonVisible(bool visible) {
            button.SetVisibleState(
                state: visible
                    ? SpawnerSelectionButtonState.HalfVisible
                    : SpawnerSelectionButtonState.FullHidden
            );
        }

        public int LocateButton(int y) {
            button.ownTransform.SetPosition(y: -y);

            return button.ownTransform.height;
        }

        public static SpawnerSelectionController Create(
            SpawnerSelectionController prefab,
            Transform parent,
            SpawnerSelectionTemplate template,
            SelectionController selectionController,
            NodeController prefabToSpawn = null
        ) {
            var newController = Instantiate(original: prefab, parent: parent);

            newController.ownTransform.depth = 0;
            newController.ownTransform.SetScale(x: 2, y: 2, z: 1);

            newController.Initialize(
                title: template.title,
                icon: template.icon,
                spawnersTemplates: template.spawnersTemplates,
                selectionController: selectionController
            );

            if (template.declarationType != "none") {
                var declareContainer = newController.container.GetComponent<DeclareContainer>();

                declareContainer.Initialize(
                    spawnLimit: template.spawnLimit,
                    declaredPrefix: template.declaredPrefix,
                    spawnerIcon: template.spawnerIcon,
                    declaredPrefab: prefabToSpawn
                );
            }

            return newController;
        }


    }
}