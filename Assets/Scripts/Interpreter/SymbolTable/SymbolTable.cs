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

        public List<string> variablesSymbols = new List<string>();
        public List<string> arraysSymbols = new List<string>();

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
                   ? variablesSymbols
                   : arraysSymbols;

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
                ? variablesSymbols
                : arraysSymbols;

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

        public List<NodeController> GetNodesWithoutParent() {
            var nodes = new List<NodeController>();

            foreach (var entry in symbols) {
                var symbol = entry.Value;

                nodes.AddRange(symbol.GetReferencesWithoutParent());
            }

            return nodes;
        }

        public Dictionary<string, string> UpdateSymbolsTemplates(SymbolTableTemplate template) {
            if (template == null) return null;

            var symbolNameChanges = new Dictionary<string, string>();

            template.arrayTemplates.ForEach(t => {
                var old = UpdateSymbolTemplate(template: t);

                symbolNameChanges[old] = t.symbolName;
            });
            template.variableTemplates.ForEach(t => {
                var old = UpdateSymbolTemplate(template: t);

                symbolNameChanges[old] = t.symbolName;
            });

            return symbolNameChanges;
        }

        string UpdateSymbolTemplate(SymbolTemplate template) {
            var oldSymbolName = template.symbolName;
            var symbolName = template.symbolName;

            var counter = 2;
            var newSymbolName = symbolName;
            while (this[newSymbolName] != null) {
                newSymbolName = $"{symbolName}({counter})";
                counter += 1;
            }

            template.symbolName = newSymbolName;

            return oldSymbolName;
        }

        public SymbolTableTemplate GetSymbolTableTemplate()
            => new SymbolTableTemplate {
                arrayTemplates = arraysSymbols.ConvertAll(s => this[s].GetSymbolTemplate()),
                variableTemplates = variablesSymbols.ConvertAll(s => this[s].GetSymbolTemplate())
            };

    }

}