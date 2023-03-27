// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class Analyzer : MonoBehaviour {

        public (bool isValid, NodeController beginning) Analize() {
            if (!IsThereABeginning()) {
                MessagesController.instance.AddMessage(
                    message: "Debe de existir un Inicio conectado a tus nodos.",
                    status: MessageStatus.Error
                );
                return (isValid: false, beginning: null);
            }

            if (!IsThereAnEnding()) {
                MessagesController.instance.AddMessage(
                    message: "Debe de existir un Final conectado a tus nodos.",
                    status: MessageStatus.Error
                );
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