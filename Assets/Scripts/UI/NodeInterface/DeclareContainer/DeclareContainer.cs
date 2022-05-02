// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;
using ScrapCoder.InputManagment;

namespace ScrapCoder.UI {
    public class DeclareContainer : MonoBehaviour {
        // Editor variables
        [SerializeField] ButtonController button;
        [SerializeField] InputText inputText;

        [SerializeField] NodeSpawnController spawnerPrefab;
        [SerializeField] NodeController declaredPrefab;

        [SerializeField] Transform temporalParent;
        [SerializeField] Transform spawnerParent;

        [SerializeField] Canvas canvas;

        [SerializeField] int spawnLimit = 10;

        [SerializeField] string declaredPrefix;

        [SerializeField] string spawnerIcon;

        [SerializeField] public SelectionCategoryContainer categoryContainer;

        // State variables
        List<NodeSpawnController> spawners = new List<NodeSpawnController>();

        // Lazy variables
        int Count => spawners.Count;
        NodeSpawnController Last => spawners.Count > 0 ? spawners[Count - 1] : null;

        // Methods
        void Start() {
            System.Action declare = () => {
                Declare();
                InputController.instance.ClearFocus();
                inputText.Clear();
            };

            button.AddListener(declare);
            inputText.AddListener(declare);
        }

        void Declare() {
            var symbolName = $"{declaredPrefix}_{inputText.Value}";
            var name = inputText.Value;

            if (name == "") {
                Debug.LogWarning($"Please input a no empty string");
                return;
            }

            if (SymbolTable.instance[symbolName] != null) {
                Debug.LogWarning($"{symbolName} already exist!");
                return;
            }

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
                discardCallback: () => DeleteDeclared(symbolName),
                template: new NodeSpawnTemplate {
                    title = name,
                    symbolName = symbolName,
                    spawnLimit = spawnLimit,
                    selectedIcon = spawnerIcon
                },
                categoryContainer: categoryContainer
            );

            SymbolTable.instance.AddSymbol(
                limit: spawnLimit,
                symbolName: symbolName,
                type: newPrefab.type,
                value: "0",
                spawner: newSpawner
            );

            PositionSpawner(newSpawner);
        }

        void PositionSpawner(NodeSpawnController spawner) {
            var lastY = Last?.ownTransform.fy - 10 ?? 0;

            spawner.ownTransform.depth = 0;
            spawner.ownTransform.SetScale(x: 2, y: 2, z: 1);
            spawner.ownTransform.SetPosition(x: 0, y: (spawner.ownTransform.height + 10) * InterfaceCanvas.OutsideFactor);
            spawner.ownTransform.SetPosition(y: lastY * InterfaceCanvas.OutsideFactor, smooth: true);

            spawners.Add(spawner);
        }

        void RepositionAllSpawners() {
            var lastY = 0;

            spawners.ForEach(s => {
                s.ownTransform.SetPosition(x: 0, y: lastY * InterfaceCanvas.OutsideFactor, smooth: true);
                lastY -= s.ownTransform.height + 10;
            });
        }

        void DeleteDeclared(string symbolName) {
            var spawner = spawners.Find(s => s.symbolName == symbolName);

            SymbolTable.instance.DeleteSymbol(symbolName);

            spawners.Remove(spawner);

            spawner.ownTransform.SetPositionByDelta(
                dx: -1000,
                smooth: true,
                endingCallback: () => Destroy(spawner.gameObject)
            );

            RepositionAllSpawners();
        }

    }
}