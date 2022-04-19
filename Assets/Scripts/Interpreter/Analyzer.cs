// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class Analyzer : MonoBehaviour {

        public (bool isValid, NodeController beginning) Analize() {
            if (!IsThereABeginning()) {
                Debug.LogError("There must be a beginning");
                return (isValid: false, beginning: null);
            }

            if (!IsThereAnEnding()) {
                Debug.LogError("There must be an ending");
                return (isValid: false, beginning: null);
            }

            var beginning = GetBeginning();

            if (!beginning.Analyze()) return (isValid: false, beginning: null);

            return (isValid: true, beginning: beginning);
        }

        bool IsThereABeginning() {
            return
                SymbolTable.instance["Begin"] != null &&
                SymbolTable.instance["Begin"].Count > 0;
        }

        bool IsThereAnEnding() {
            return
                SymbolTable.instance["End"] != null &&
                SymbolTable.instance["End"].Count > 0;
        }

        NodeController GetBeginning() {
            return SymbolTable.instance["Begin"].first;
        }
    }
}