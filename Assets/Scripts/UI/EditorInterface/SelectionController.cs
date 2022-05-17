// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionController : MonoBehaviour {

        // Editor variables
        [SerializeField] Transform selectionParent;

        [SerializeField] SpawnerSelectionController selectionPrefab;
        [SerializeField] SpawnerSelectionController selectionWithDeclarationPrefab;

        [SerializeField] SpawnerSelectionController selectionWithTables;

        [SerializeField] NodeController variablePrefab;
        [SerializeField] NodeController arrayPrefab;

        // State variables
        List<SpawnerSelectionController> selectionSpawnersControllers;

        bool initialized = false;

        public SpawnerSelectionController variablesSelection;
        public SpawnerSelectionController arraysSelection;

        // Methods
        public void Initialize(List<SpawnerSelectionTemplate> selectionTemplates) {
            if (initialized) return;

            selectionSpawnersControllers = CreateSelectionSpawners(selectionTemplates);

            selectionWithTables.selectionController = this;
            selectionSpawnersControllers.Add(selectionWithTables);

            PositionAndOrderButtons();

            var newHeight = (InterfaceCanvas.instance.rectDimensions.y - 153) / 2;
            ExpandContainers(newHeight: newHeight);

            initialized = true;
        }

        List<SpawnerSelectionController> CreateSelectionSpawners(List<SpawnerSelectionTemplate> selectionTemplates) {
            return selectionTemplates.ConvertAll(
                template => {
                    var selectionSpawner = SpawnerSelectionController.Create(
                        prefab: template.declarationType == "none"
                            ? selectionPrefab
                            : selectionWithDeclarationPrefab,
                        parent: selectionParent,
                        template: template,
                        selectionController: this,
                        prefabToSpawn: template.declarationType == "none"
                            ? null
                            : template.declarationType == "variable"
                            ? variablePrefab
                            : arrayPrefab
                    );

                    if (template.declarationType == "variable") {
                        variablesSelection = selectionSpawner;
                    } else if (template.declarationType == "array") {
                        arraysSelection = selectionSpawner;
                    }

                    return selectionSpawner;
                }
            );
        }

        void PositionAndOrderButtons() {
            var y = 5;
            var menuOrder = HierarchyController.instance.lastNodesOrder + 10;

            selectionSpawnersControllers.ForEach(s => {
                y += s.PositionButton(y) + 5;
                s.SetOrder(order: menuOrder);
            });
        }

        public void ExpandContainers(int newHeight) {
            selectionSpawnersControllers?.ForEach(s => {
                s.container.ownTransform.ExpandByNewDimensions(
                    newHeight: newHeight
                );
            });
        }

        public void HideAllButtons() {
            selectionSpawnersControllers.ForEach(s => s.SetButtonVisible(visible: false));
        }

        public void ShowAllButtons() {
            selectionSpawnersControllers.ForEach(s => s.SetButtonVisible(visible: true));
        }

        public void SetSelectionMenusOrderByDelta(int delta) {
            selectionSpawnersControllers.ForEach(s => s.SetOrderByDelta(delta));
        }

    }
}