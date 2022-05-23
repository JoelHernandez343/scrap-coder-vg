// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ModifyVariableBuilder : InterpreterElementBuilder {

        // Internal types
        enum Operation { Add, Decrease }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] Operation operation;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController variable => variableContainer.array.First;
        string symbolName => variable.symbolName;

        int variableValue {
            get => System.Int32.Parse(SymbolTable.instance[symbolName].Value);
            set => SymbolTable.instance[symbolName].SetValue(newValue: $"{value}");
        }

        public override void Execute(string argument) {

            if (operation == Operation.Add) {
                variableValue = variableValue + 1;
            } else if (operation == Operation.Decrease) {
                variableValue = variableValue - 1;
            }

            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

    }
}