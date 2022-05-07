// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

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

        NodeSpawnController spawner;

        ValueTableController table;
        public ValueTableController Table => table;

        // Lazy variables
        public bool isFull => limit > 0 && references.Count == limit;

        public int Count => references.Count;

        public NodeController first => Count == 0 ? null : references[0];

        // Methods
        public Symbol(string symbolName, NodeType type, string initValue, int limit, NodeSpawnController spawner, ValueTableController table = null) {
            this.type = type;
            this.limit = limit;
            this.table = table;
            this.value = initValue;
            this.spawner = spawner;
            this.symbolName = symbolName;
        }

        public void AddReference(NodeController reference) {
            if (isFull) throw new System.OverflowException($"The symbol has reached its limit: {limit}");

            references.Add(reference);
        }

        public void RemoveReference(NodeController reference) {
            if (references.Remove(reference)) {
                spawner.RefreshCounter();
            }
        }

        public void RemoveAllReferences(bool removeChildren) {
            references.ForEach(r => r.RemoveMyself(removeChildren: removeChildren));
            references.Clear();

            spawner.RefreshCounter();

            value = "";
        }

        public void RemoveReferencesWithoutParent() {
            references.FindAll(r => !r.hasParent).ForEach(r => {
                r.RemoveMyself(removeChildren: true);
                RemoveReference(r);
            });
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

            if (ArrayLength == 100) {
                throw new System.OverflowException($"Array has reached its limit");
            }

            arrayOfValues.Add(value);
            table.AddRow(value);
        }

        public void InsertToArray(int index, string value) {
            CheckArray();

            if (index < 0 || index >= 100) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }
            if (ArrayLength == 100) {
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

            if (index < 0 || index >= ArrayLength) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }

            arrayOfValues[index] = newValue;
            table.ChangeRowValue(index: index, newValue: newValue);
        }

    }
}