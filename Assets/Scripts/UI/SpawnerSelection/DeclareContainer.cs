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

        NodeType type => declaredPrefab.type;

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
                MessagesController.instance.AddMessage(
                    message: "Ingresa un nombre no vacío.",
                    type: MessageType.Error
                );
                return;
            }

            if (!SymbolNameHandler.validForm.IsMatch(rawValue)) {
                MessagesController.instance.AddMessage(
                    message: "Ingresa un nombre que empiece con una letra.",
                    type: MessageType.Error
                );
                return;
            }

            Declare(symbolName: symbolName, smooth: true);
        }

        public bool Declare(string symbolName, bool smooth = false, SymbolTemplate template = null) {
            if (selectionContainer.SpawnersCount == declarationLimit) {
                MessagesController.instance.AddMessage(
                    message: $"Ya no puedes declarar más variables, el límite es de: {declarationLimit}.",
                    type: MessageType.Warning
                );
                return false;
            }

            if (SymbolTable.instance[symbolName] != null) {
                MessagesController.instance.AddMessage(
                    message: $"{(type == NodeType.Variable ? "La variable" : "El arreglo")}: {symbolName} ya existe. No lo puedes declarar dos veces",
                    type: MessageType.Error
                );
                return false;
            }

            var name = symbolName.Split(new char[] { '_' })[1];

            var newPrefab = NodeController.Create(
                prefab: declaredPrefab,
                parent: temporalParent,
                template: new NodeControllerTemplate {
                    name = name,
                    symbolName = symbolName,
                    customInfo = new Dictionary<string, object> {
                        ["nameText"] = name
                    }
                },
                isPrototype: true
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

            return true;
        }

    }
}