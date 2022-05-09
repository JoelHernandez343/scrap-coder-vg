// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ClearArrayInterpreter : InterpreterElement {

        // Editor variables
        [SerializeField] NodeContainer arrayContainer;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController array => arrayContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public override void Execute(string argument) {
            SymbolTable.instance[symbolName].ClearArray();
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

    }
}