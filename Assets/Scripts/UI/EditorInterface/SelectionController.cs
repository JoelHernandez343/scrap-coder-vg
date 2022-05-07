// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionController : MonoBehaviour {

        // Editor variables
        [SerializeField] Transform categoriesParent;

        [SerializeField] SpawnerSelectionController categoryPrefab;
        [SerializeField] SpawnerSelectionController categoryWithDeclarationPrefab;

        [SerializeField] SpawnerSelectionController categoryWithTables;

        [SerializeField] NodeController variablePrefab;
        [SerializeField] NodeController arrayPrefab;

        // State variables
        List<SpawnerSelectionController> categoryControllers;

        bool initialized = false;

        // Methods
        public void Initialize(List<SpawnerSelectionTemplate> categoryTemplates = null) {
            if (initialized) return;

            categoryControllers = categoryTemplates.ConvertAll(
                template => SpawnerSelectionController.Create(
                    prefab: template.declarationType == "none"
                        ? categoryPrefab
                        : categoryWithDeclarationPrefab,
                    parent: categoriesParent,
                    template: template,
                    selectionController: this,
                    prefabToSpawn: template.declarationType == "none"
                        ? null
                        : template.declarationType == "variable"
                        ? variablePrefab
                        : arrayPrefab
                )
            );

            categoryWithTables.selectionController = this;

            categoryControllers.Add(categoryWithTables);

            LocateButtons();

            initialized = true;
        }

        void LocateButtons() {
            var y = 10;

            categoryControllers.ForEach(c => y += c.LocateButton(y) + 10);
        }

        public void HideAllButtons() {
            categoryControllers.ForEach(c => c.SetButtonVisible(visible: false));
        }

        public void ShowAllButtons() {
            categoryControllers.ForEach(c => c.SetButtonVisible(visible: true));
        }

    }
}