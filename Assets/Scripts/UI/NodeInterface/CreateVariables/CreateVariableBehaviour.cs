// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;
using ScrapCoder.InputManagment;

namespace ScrapCoder.UI {
    public class CreateVariableBehaviour : MonoBehaviour {
        // Editor variables
        [SerializeField] ButtonController button;
        [SerializeField] InputText inputText;

        [SerializeField] NodeSpawnController variableSpawnerPrefab;
        [SerializeField] NodeController variablePrefab;

        [SerializeField] Transform variableTemporalParent;
        [SerializeField] Transform spawnerParent;

        [SerializeField] Canvas canvas;

        [SerializeField] int spawnLimit = 10;

        // State variables
        public List<NodeSpawnController> spawners = new List<NodeSpawnController>();

        // Methods
        void Start() {
            button.AddListener(() => {
                CreateVariable();
                InputController.instance.ClearFocus();
                inputText.Clear();
            });
            inputText.AddListener(() => {
                CreateVariable();
                InputController.instance.ClearFocus();
                inputText.Clear();
            });
        }

        void CreateVariable() {

            var variableSymbolName = $"variable_{inputText.Value}";
            var variableName = inputText.Value;

            if (variableSymbolName == "variable_") {
                Debug.LogWarning($"Input not empty string");
                return;
            }

            if (SymbolTable.instance[variableSymbolName] != null) {
                Debug.LogWarning($"{variableSymbolName} already exist!");
                return;
            }

            var prefab = CreatePrefab(variableName, variableSymbolName);

            var newSpawner = CreateSpawn(
                prefab: prefab,
                variableName: variableName,
                variableSymbolName: variableSymbolName
            );

            PositionSpawner(newSpawner);
        }

        void PositionSpawner(NodeSpawnController spawner) {
            spawner.ownTransform.SetScale(x: 2, y: 2);
            spawner.ownTransform.depth = 0;

            var lastPosition = Vector2Int.zero;

            if (spawners.Count > 0) {
                var last = spawners[spawners.Count - 1];
                lastPosition.y = last.ownTransform.fy - 10;
            }

            spawner.ownTransform.SetPosition(x: 0, y: spawner.ownTransform.height + 10);
            spawner.ownTransform.SetPosition(
                x: lastPosition.x,
                y: lastPosition.y,
                smooth: true
            );

            spawners.Add(spawner);
        }

        void RepositionAllSpawners() {
            var lastPosition = Vector2Int.zero;

            spawners.ForEach(s => {
                s.ownTransform.SetPosition(
                    x: lastPosition.x,
                    y: lastPosition.y
                );

                lastPosition.y = s.ownTransform.fy - 10;
            });
        }

        void DeleteVariable(NodeSpawnController spawn) {
            var symbolName = spawn.symbolName;

            SymbolTable.instance.RemoveSymbol(symbolName);

            spawners.Remove(spawn);

            Destroy(spawn.gameObject);

            RepositionAllSpawners();
        }

        NodeController CreatePrefab(string variableName, string variableSymbolName) {
            var newPrefab = Instantiate(variablePrefab);
            var newPrefabVariable = (newPrefab.GetComponent<NodeControllerVariable>() as NodeControllerVariable);

            newPrefabVariable.name = variableName;
            newPrefabVariable.text = variableName;
            newPrefabVariable.hideAfterExpand = true;
            newPrefabVariable.temporalParent = variableTemporalParent;

            SymbolTable.instance.AddSymbol(
                limit: spawnLimit,
                symbolName: variableSymbolName,
                type: NodeType.Variable,
                value: "0"
            );

            return newPrefab;
        }

        NodeSpawnController CreateSpawn(NodeController prefab, string variableName, string variableSymbolName) {
            var newSpawner = Instantiate(original: variableSpawnerPrefab, parent: spawnerParent);

            newSpawner.text = variableName;
            newSpawner.symbolName = variableSymbolName;
            newSpawner.prefab = prefab;
            newSpawner.canvas = canvas;
            newSpawner.limit = spawnLimit;

            (newSpawner.GetComponent<VariableSpawnController>() as VariableSpawnController)
                ?.deleteButton.AddListener(() => DeleteVariable(newSpawner));

            return newSpawner;
        }
    }
}