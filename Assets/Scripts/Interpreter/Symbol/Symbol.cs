// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;
using ScrapCoder.Tutorial;

namespace ScrapCoder.Interpreter {

    public class Symbol {
        public int limit;

        List<NodeController> references = new List<NodeController>();

        NodeType type;
        public NodeType Type => type;

        string value;
        public string Value => value;

        string symbolName;
        public string SymbolName;

        List<string> arrayOfValues = new List<string>();

        NodeSpawnController _spawner;
        public NodeSpawnController spawner {
            get => _spawner;
            private set => _spawner = value;
        }

        ValueTableController table;
        public ValueTableController Table => table;

        // Lazy variables
        public bool isFull => limit > 0 && Count == limit;

        public int Count => references.Count;

        public NodeController first => Count == 0 ? null : references[0];

        public const int ArrayLimit = 20;

        // Methods
        public Symbol(
            string symbolName,
            NodeType type,
            int limit,
            NodeSpawnController spawner,
            ValueTableController table,
            string initValue,
            List<string> initialArrayValues
        ) {
            this.type = type;
            this.limit = limit;
            this.table = table;
            this.spawner = spawner;
            this.symbolName = symbolName;

            if (type == NodeType.Variable) {
                SetValue(newValue: initValue);
            } else if (type == NodeType.Array) {
                initialArrayValues.ForEach(value => AddToArray(value: value));
            }
        }

        public void AddReference(NodeController reference) {
            if (isFull) throw new System.OverflowException($"The symbol has reached its limit: {limit}");

            TutorialController.instance.ReceiveSignal(signal: $"spawnedType{type}");

            references.Add(reference);
            spawner.RefreshCounter();
        }

        public void RemoveReference(NodeController reference) {
            if (references.Remove(reference)) {
                spawner.RefreshCounter();
                TutorialController.instance.ReceiveSignal(signal: $"removedType{type}");
            }
        }

        public void DeleteAllNodes(bool deleteChildren = false) {
            var referencesToRemove = new List<NodeController>(references);

            referencesToRemove
                .ForEach(r => r.DeleteSelf(deleteChildren: deleteChildren));
        }

        public void DeleteNodesWithoutParent() {
            references
                .FindAll(r => !r.hasParent)
                .ForEach(r => r.DeleteSelf(deleteChildren: true));
        }

        // Variables methods
        public void SetValue(string newValue) {
            value = newValue;
            table.ChangeDescription(value);
        }

        // Array methods
        public int ArrayLength => arrayOfValues.Count;

        void CheckArray() {
            if (type != NodeType.Array) {
                throw new System.TypeAccessException($"{symbolName} is not an array");
            }
        }

        public void ClearArray() {
            arrayOfValues.Clear();
            table.ClearAllRows();
        }

        public void AddToArray(string value) {
            CheckArray();

            if (ArrayLength == ArrayLimit) {
                throw new System.OverflowException($"Array has reached its limit");
            }

            arrayOfValues.Add(value);
            table.AddRow(value);
        }

        public void InsertToArray(int index, string value) {
            CheckArray();

            if (index < 0 || index >= ArrayLimit) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }
            if (ArrayLength == ArrayLimit) {
                throw new System.OverflowException($"Array has reached its limit");
            }

            if (index > ArrayLength) {
                var missing = index - ArrayLength;

                for (var i = 0; i < missing; ++i) {
                    arrayOfValues.Add("0");
                    table.AddRow("0");
                }
            }

            arrayOfValues.Insert(index, value);
            table.InsertRowAt(index: index, value: value);
        }

        public void RemoveFromArray(int index) {
            CheckArray();

            if (index < 0 || index >= ArrayLength) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }

            arrayOfValues.RemoveAt(index);
            table.RemoveRowAt(index);
        }

        public string GetValueFromArray(int index) {
            CheckArray();

            if (index < 0 || index >= ArrayLength) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }

            return arrayOfValues[index];
        }

        public void SetValueInArray(int index, string newValue) {
            CheckArray();

            if (index < 0) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }

            if (index >= ArrayLength) {
                InsertToArray(index: index, value: newValue);
                return;
            }

            arrayOfValues[index] = newValue;
            table.ChangeRowValue(index: index, newValue: newValue);
        }

        public void SortArrayAsNumber() {
            arrayOfValues.Sort((a, b) => {
                var numberA = System.Int32.Parse(a);
                var numberB = System.Int32.Parse(b);

                return numberA.CompareTo(numberB);
            });

            for (int i = 0; i < ArrayLength; ++i) {
                table.ChangeRowValue(index: i, newValue: $"{arrayOfValues[i]}");
            }
        }

    }
}