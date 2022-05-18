// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;
using ScrapCoder.GameInput;

namespace ScrapCoder.UI {
    public class DeclareContainer : MonoBehaviour {
        // Editor variables
        [SerializeField] ButtonController button;
        [SerializeField] InputText inputText;

        [SerializeField] Transform temporalParent;
        [SerializeField] Transform spawnerParent;

        [SerializeField] NodeSpawnController spawnerPrefab;

        [SerializeField] int spawnLimit = 10;
        [SerializeField] string declaredPrefix;
        [SerializeField] string spawnerIcon;
        [SerializeField] int declarationLimit;

        // State variables
        NodeController declaredPrefab;

        // Lazy variables
        SpawnerSelectionContainer _selectionContainer;
        SpawnerSelectionContainer selectionContainer
            => _selectionContainer
                ??= (GetComponent<SpawnerSelectionContainer>() as SpawnerSelectionContainer);

        // Methods
        public void Initialize(
            int spawnLimit,
            string declaredPrefix,
            string spawnerIcon,
            NodeController declaredPrefab,
            int declarationLimit
        ) {
            // Copy of data
            this.spawnLimit = spawnLimit;
            this.declaredPrefix = declaredPrefix;
            this.spawnerIcon = spawnerIcon;
            this.declaredPrefab = declaredPrefab;
            this.declarationLimit = declarationLimit;

            System.Action declare = () => {
                DeclareFromInputText();
                InputController.instance.ClearFocus();
                inputText.Clear();
            };

            button.AddListener(declare);
            inputText.AddListener(declare);
        }

        void DeclareFromInputText() {
            var rawValue = inputText.Value;
            var symbolName = $"{declaredPrefix}_{rawValue}";

            if (rawValue == "") {
                Debug.LogWarning($"Please input a no empty string");
                return;
            }

            if (!SymbolNameHandler.validForm.IsMatch(rawValue)) {
                Debug.LogWarning($"Please input a string starting with a letter");
                return;
            }

            Declare(symbolName: symbolName, smooth: true);
        }

        public void Declare(string symbolName, bool smooth = false, SymbolTemplate template = null) {
            if (selectionContainer.SpawnersCount == declarationLimit) {
                Debug.LogWarning("Cannot declare more!");
                return;
            }

            if (SymbolTable.instance[symbolName] != null) {
                Debug.LogWarning($"{symbolName} already exist!");
                return;
            }

            var name = symbolName.Split(new char[] { '_' })[1];

            var newPrefab = NodeControllerExpandableByText.Create(
                prefab: declaredPrefab,
                parent: temporalParent,
                name: name,
                symbolName: symbolName
            );

            var newSpawner = NodeSpawnController.Create(
                spawnerPrefab: spawnerPrefab,
                parent: spawnerParent,
                prefabToSpawn: newPrefab,
                discardCallback: () => selectionContainer.RemoveSpawner(symbolName: symbolName, smooth: true),
                template: new NodeSpawnTemplate {
                    title = name,
                    symbolName = symbolName,
                    spawnLimit = spawnLimit,
                    selectedIcon = spawnerIcon
                },
                categoryContainer: selectionContainer
            );

            SymbolTable.instance.AddSymbol(
                limit: spawnLimit,
                symbolName: symbolName,
                type: newPrefab.type,
                spawner: newSpawner,
                value: template?.value ?? "0",
                arrayValues: template?.arrayValues ?? new List<string>()
            );

            selectionContainer.AddSpawner(spawner: newSpawner, smooth: smooth);
        }

    }
}