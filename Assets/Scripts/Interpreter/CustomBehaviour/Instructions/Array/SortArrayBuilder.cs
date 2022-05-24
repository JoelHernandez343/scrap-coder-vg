// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class SortArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new SortArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                array: arrayContainer.First
            );
        }

    }

    class SortArrayInterpreter : InterpreterElement {

        // State variables
        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => false;

        // Methods
        public override void Execute(string argument) {
            SymbolTable.instance[arraySymbolName].SortArrayAsNumber();
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        public SortArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeController array
        ) : base(parentList, controllerReference) {

            arraySymbolName = array.symbolName;

        }

    }
}