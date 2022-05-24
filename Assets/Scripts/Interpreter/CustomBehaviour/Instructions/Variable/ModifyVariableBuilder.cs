// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ModifyVariableBuilder : InterpreterElementBuilder {

        // Internal types
        public enum Operation { Add, Decrease }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] Operation operation;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ModifyVariableInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                variableContainer: variableContainer,
                operation: operation
            );
        }

    }

    class ModifyVariableInterpreter : InterpreterElement {

        // State variables
        ModifyVariableBuilder.Operation operation;

        List<InterpreterElement> variableList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement variable => variableList[0];
        string variableSymbolName => variable.symbolName;

        int variableValue {
            get => System.Int32.Parse(SymbolTable.instance[variableSymbolName].Value);
            set => SymbolTable.instance[variableSymbolName].SetValue(newValue: $"{value}");
        }

        public override void Execute(string argument) {

            if (operation == ModifyVariableBuilder.Operation.Add) {
                variableValue = variableValue + 1;
            } else if (operation == ModifyVariableBuilder.Operation.Decrease) {
                variableValue = variableValue - 1;
            }

            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        public ModifyVariableInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer variableContainer,
            ModifyVariableBuilder.Operation operation
        ) : base(parentList, controllerReference) {

            variableList.AddRange(InterpreterElementsFromContainer(
                container: variableContainer,
                parentList: variableList
            ));

            this.operation = operation;

        }

    }
}