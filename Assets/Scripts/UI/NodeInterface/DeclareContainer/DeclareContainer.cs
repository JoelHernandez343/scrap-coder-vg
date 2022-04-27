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

            var newPrefab = CreatePrefab(
                name: name,
                symbolName: symbolName
            );

            var newSpawner = CreateSpawn(
                prefab: newPrefab,
                name: name,
                symbolName: symbolName
            );

            PositionSpawner(newSpawner);
        }

        void PositionSpawner(NodeSpawnController spawner) {
            var lastY = Last?.ownTransform.fy - 10 ?? 0;

            spawner.ownTransform.depth = 0;
            spawner.ownTransform.SetScale(x: 2, y: 2, z: 1);
            spawner.ownTransform.SetPosition(x: 0, y: spawner.ownTransform.height + 10);
            spawner.ownTransform.SetPosition(y: lastY, smooth: true);

            spawners.Add(spawner);
        }

        void RepositionAllSpawners() {
            var lastY = 0;

            spawners.ForEach(s => {
                s.ownTransform.SetPosition(x: 0, y: lastY, smooth: true);
                lastY -= s.ownTransform.height + 10;
            });
        }

        void DeleteVariable(NodeSpawnController spawn) {
            SymbolTable.instance.RemoveSymbol(spawn.symbolName);

            spawners.Remove(spawn);

            Destroy(spawn.gameObject);

            RepositionAllSpawners();
        }

        NodeController CreatePrefab(string name, string symbolName) {
            var newPrefab = Instantiate(declaredPrefab);
            var newPrefabExpandable = (newPrefab.GetComponent<NodeControllerExpandableByText>() as NodeControllerExpandableByText);

            newPrefab.symbolName = symbolName;

            newPrefabExpandable.name = name;
            newPrefabExpandable.text = name;
            newPrefabExpandable.hideAfterExpand = true;
            newPrefabExpandable.temporalParent = temporalParent;

            SymbolTable.instance.AddSymbol(
                limit: spawnLimit,
                symbolName: symbolName,
                type: newPrefab.type,
                value: "0"
            );

            return newPrefab;
        }

        NodeSpawnController CreateSpawn(NodeController prefab, string name, string symbolName) {
            var newSpawner = Instantiate(original: spawnerPrefab, parent: spawnerParent);

            newSpawner.name = $"spawner_{symbolName}";
            newSpawner.symbolName = symbolName;
            newSpawner.limit = spawnLimit;
            newSpawner.canvas = canvas;
            newSpawner.prefab = prefab;
            newSpawner.text = name;

            (newSpawner.GetComponent<DeclaredSpawnController>() as DeclaredSpawnController)
                ?.deleteButton?.AddListener(() => DeleteVariable(newSpawner));

            return newSpawner;
        }
    }
}