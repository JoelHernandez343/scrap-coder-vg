// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ClearArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ClearArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                array: arrayContainer.First
            );
        }

    }

    class ClearArrayInterpreter : InterpreterElement {

        // State variables
        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => false;

        // Methods
        public override void Execute(string argument) {
            SymbolTable.instance[arraySymbolName].ClearArray();
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        public ClearArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeController array
        ) : base(parentList, controllerReference) {

            arraySymbolName = array.symbolName;

        }

    }
}