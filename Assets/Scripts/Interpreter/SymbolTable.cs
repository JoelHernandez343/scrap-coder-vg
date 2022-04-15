using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class SymbolTable : MonoBehaviour {
        public static SymbolTable instance;

        // State variables
        Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

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

        public void AddSymbol(int limit, string symbolName, NodeType type) {
            symbols[symbolName] = new Symbol {
                limit = limit,
                type = type
            };
        }

    }

}