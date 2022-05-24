// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class SymbolTable : MonoBehaviour {

        // Static variables 
        public static SymbolTable instance;

        // Editor variables
        [SerializeField] ValueTablesContainer tablesContainer;

        // State variables
        Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        public List<string> variables_symbols = new List<string>();
        public List<string> arrays_symbols = new List<string>();

        void Awake() {
            if (instance != null) {
                Destroy(this);
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

        public void AddSymbol(
            int limit,
            string symbolName,
            NodeType type,
            NodeSpawnController spawner,
            string value = "",
            List<string> arrayValues = null
        ) {

            ValueTableController table = null;

            if (type == NodeType.Variable || type == NodeType.Array) {

                var list = type == NodeType.Variable
                   ? variables_symbols
                   : arrays_symbols;

                table = tablesContainer.AddElement(
                    symbolName: symbolName,
                    description: type == NodeType.Variable ? value : "<vacío>",
                    nodeType: type
                );

                list.Add(symbolName);
            }

            symbols[symbolName] = new Symbol(
                type: type,
                limit: limit,
                initValue: value,
                spawner: spawner,
                symbolName: symbolName,
                table: table,
                initialArrayValues: arrayValues
            );
        }

        public bool DeleteSymbol(string symbolName) {
            var symbol = this[symbolName];
            if (symbol == null) return false;

            symbol.DeleteAllNodes();
            symbols.Remove(symbolName);

            var list = symbol.Type == NodeType.Variable
                ? variables_symbols
                : arrays_symbols;

            list.Remove(symbolName);
            tablesContainer.RemoveElement(
                tableToRemove: symbol.Table,
                nodeType: symbol.Type
            );

            return true;
        }

        public void DeleteAllNodesWihoutParent() {
            foreach (var entry in symbols) {
                var symbol = entry.Value;

                symbol.DeleteNodesWithoutParent();
            }
        }

    }

}