// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class SymbolTable : MonoBehaviour {
        public static SymbolTable instance;

        // State variables
        Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        List<string> variables = new List<string>();
        public List<string> Variables => new List<string>(variables);

        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public Symbol this[string symbol] {
            get {
                try {
                    return symbols[symbol];
                } catch (KeyNotFoundException) {
                    return null;
                }
            }
        }

        public void AddSymbol(int limit, string symbolName, NodeType type, NodeSpawnController spawner, string value = "") {
            symbols[symbolName] = new Symbol {
                type = type,
                limit = limit,
                value = value,
                spawner = spawner,
                symbolName = symbolName
            };

            if (type == NodeType.Variable) {
                variables.Add(symbolName);
            }
        }

        public bool DeleteSymbol(string symbolName) {
            var symbol = this[symbolName];
            if (symbol == null) return false;

            symbol.RemoveAllReferences(removeChildren: true);
            symbols.Remove(symbolName);

            if (symbol.type == NodeType.Variable) {
                variables.Remove(symbolName);
            }

            return true;
        }

        public void CleanReferencesWihoutParent() {
            foreach (var entry in symbols) {
                var symbol = entry.Value;

                symbol.RemoveReferencesWithoutParent();
            }
        }

    }

}