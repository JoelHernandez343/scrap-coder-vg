// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class Symbol {
        public int limit;

        List<NodeController> references = new List<NodeController>();

        public NodeType type;

        public string value;

        public string symbolName;

        public List<string> arrayOfValues = new List<string>();

        // Lazy variables
        public bool isFull => limit > 0 && references.Count == limit;

        public int Count => references.Count;

        public NodeController first => Count == 0 ? null : references[0];

        // Methods
        public void Add(NodeController reference) {
            if (isFull) throw new System.OverflowException($"The symbol has reached its limit: {limit}");

            references.Add(reference);
        }

        public void Remove(NodeController reference) {
            references.Remove(reference);
        }

        public void RemoveAllReferences() {
            references.ForEach(r => r.RemoveMyself());
            references.Clear();

            value = "";
        }

        public void RemoveReferencesWithoutParent() {
            references.FindAll(r => !r.hasParent).ForEach(r => {
                r.RemoveMyself();
                Remove(r);
            });
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
        }

        public void AddToArray(string value) {
            CheckArray();

            if (ArrayLength == 100) {
                throw new System.OverflowException($"Array has reached its limit");
            }

            arrayOfValues.Add(value);
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
                var missing = index - ArrayLength - 1;

                for (var i = 0; i < missing; ++i) {
                    arrayOfValues.Add("0");
                }
            }

            arrayOfValues.Insert(index, value);
        }

        public void RemoveFromArray(int index) {
            CheckArray();

            if (index < 0 || index >= ArrayLength) {
                throw new System.IndexOutOfRangeException($"{index} is out of bounds");
            }

            arrayOfValues.RemoveAt(index);
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
        }

    }
}