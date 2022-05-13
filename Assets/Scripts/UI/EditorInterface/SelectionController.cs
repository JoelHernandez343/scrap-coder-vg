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

        // Methods
        public void Initialize(List<SpawnerSelectionTemplate> selectionTemplates = null) {
            if (initialized) return;

            selectionSpawnersControllers = selectionTemplates.ConvertAll(
                template => SpawnerSelectionController.Create(
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
                )
            );

            selectionWithTables.selectionController = this;

            selectionSpawnersControllers.Add(selectionWithTables);

            PositionAndOrderButtons();

            initialized = true;
        }

        void PositionAndOrderButtons() {
            var y = 3;
            var menuOrder = HierarchyController.instance.lastNodesOrder + 10;

            selectionSpawnersControllers.ForEach(s => {
                y += s.PositionButton(y) + 3;
                s.SetOrder(order: menuOrder);
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