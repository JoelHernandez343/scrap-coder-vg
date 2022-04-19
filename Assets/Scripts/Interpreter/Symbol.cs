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

        public void RemoveAll() {
            references.ForEach(r => r.RemoveMyself());
            references.Clear();

            value = "";
        }
    }

}