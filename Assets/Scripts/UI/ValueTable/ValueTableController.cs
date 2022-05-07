// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Interpreter;
using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ValueTableController : MonoBehaviour {

        // Editor variables
        [SerializeField] Transform rowsParent;

        [SerializeField] ExpandableText titleText;
        [SerializeField] ExpandableText descriptionText;

        [SerializeField] ValueRowController rowPrefab;

        // State variables
        public string symbolName;

        public NodeType nodeType;
        ValueTablesContainer container;

        List<ValueRowController> rows = new List<ValueRowController>();

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool smoothExpanding => Executer.instance.velocity != ExecuterVelocity.Immediately;

        const int borderOffset = 1;

        // Methods
        public void Initialize(string symbolName, string description) {
            this.symbolName = symbolName;

            titleText.ChangeText(newText: symbolName);
            descriptionText.ChangeText(newText: description);
        }

        public void ChangeDescription(string newDescription) {
            descriptionText.ChangeText(newText: newDescription);
        }

        // Arrow methods
        public void InsertRowAt(int index, string value) {
            var newRow = ValueRowController.Create(
                prefab: rowPrefab,
                parent: rowsParent,
                index: index,
                value: value
            );

            rows.Insert(
                index: index,
                item: newRow
            );

            var dy = rows.Count == 1 ? 0 : newRow.ownTransform.height - borderOffset;

            AdjustDimensions(dy: dy);
        }

        public void RemoveRowAt(int index) {
            var deletedRow = rows[index];
            rows.RemoveAt(index);

            var dy = rows.Count == 0 ? 0 : deletedRow.ownTransform.height - borderOffset;

            AdjustDimensions(dy: -dy);
        }

        void PositionRows(bool smooth = false) {
            var y = 0;

            rows.ForEach(row => {
                row.ownTransform.SetPosition(x: 0, y: -y, smooth: smooth);
                y += row.ownTransform.height - borderOffset;
            });
        }

        void AdjustDimensions(int dy) {
            PositionRows();

            ownTransform.Expand(dy: dy, smooth: smoothExpanding);
            container.Adjust(dy: dy, nodeType: nodeType);
        }

        public static ValueTableController Create(
            ValueTableController prefab,
            Transform parent,
            string symbolName,
            string description,
            NodeType nodeType,
            ValueTablesContainer container
        ) {
            var newTable = Instantiate(original: prefab, parent: parent);

            newTable.ownTransform.depth = 0;
            newTable.ownTransform.SetScale(x: 1, y: 1, z: 1);

            newTable.nodeType = nodeType;
            newTable.container = container;
            newTable.Initialize(symbolName: symbolName, description: description);

            return newTable;
        }

    }
}