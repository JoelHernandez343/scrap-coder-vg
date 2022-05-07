// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Interpreter;
using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ValueTablesContainer : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeTransform variablesContainer;
        [SerializeField] NodeTransform variablesParent;

        [SerializeField] NodeTransform arraysContainer;
        [SerializeField] NodeTransform arraysParent;

        [SerializeField] ValueTableController tablePrefab;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        bool smoothExpanding => Executer.instance.velocity != ExecuterVelocity.Immediately;

        SpawnerSelectionContainer _selectionContainer;
        SpawnerSelectionContainer selectionContainer => _selectionContainer ??= (GetComponent<SpawnerSelectionContainer>() as SpawnerSelectionContainer);

        NodeTransform content => selectionContainer.content;
        ScrollBarController scrollBar => selectionContainer.scrollBar;

        const int borderOffset = 1;

        // State variables
        public List<ValueTableController> variableTables = new List<ValueTableController>();
        public List<ValueTableController> arrayTables = new List<ValueTableController>();

        // Methods
        public ValueTableController AddElement(string symbolName, string description, NodeType nodeType) {
            var tablesList = nodeType == NodeType.Variable
                ? variableTables
                : arrayTables;

            var parent = nodeType == NodeType.Variable
                ? variablesParent.transform
                : arraysParent.transform;

            var newTable = ValueTableController.Create(
                prefab: tablePrefab,
                parent: parent,
                symbolName: symbolName,
                description: description,
                nodeType: nodeType,
                container: this
            );

            tablesList.Add(newTable);

            var dy = newTable.ownTransform.height - (tablesList.Count == 1 ? 0 : borderOffset);

            Adjust(dy: dy, nodeType: nodeType);

            return newTable;
        }

        public void RemoveElement(ValueTableController tableToRemove, NodeType nodeType) {
            var tablesList = nodeType == NodeType.Variable
                ? variableTables
                : arrayTables;

            tablesList.Remove(tableToRemove);

            Destroy(tableToRemove.gameObject);

            var dy = tableToRemove.ownTransform.height - (tablesList.Count == 0 ? 0 : borderOffset);

            Adjust(dy: -dy, nodeType: nodeType);
        }

        int PositionTables(List<ValueTableController> tables) {
            var y = 0;

            tables.ForEach(table => {
                table.ownTransform.SetPosition(x: 0, y: -y, smooth: smoothExpanding);
                y += table.ownTransform.height - borderOffset;
            });

            return y;
        }

        public void Adjust(int dy, NodeType nodeType) {
            var tablesList = nodeType == NodeType.Variable
                ? variableTables
                : arrayTables;

            var container = nodeType == NodeType.Variable
                ? variablesContainer
                : arraysContainer;

            var nextContainer = nodeType == NodeType.Variable
                ? arraysContainer
                : null;

            PositionTables(tablesList);

            container.Expand(dy: dy);
            nextContainer?.SetPositionByDelta(dy: -dy, smooth: smoothExpanding);


            var newY = -arraysContainer.fy + 24;
            Debug.Log($"FinalY of array: {newY}");

            if (newY >= content.initHeight) {
                content.Expand(dy: newY - content.height);
            } else {
                content.Expand(dy: -(content.height - content.initHeight));
            }

            scrollBar.RefreshSlider();
        }

    }
}