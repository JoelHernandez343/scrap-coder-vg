// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class GetLengthOfArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new GetLengthOfArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                array: arrayContainer.First
            );
        }

    }

    class GetLengthOfArrayInterpreter : InterpreterElement {

        // State variables
        string arraysymbolName;

        // Lazy variables
        public override bool isExpression => true;

        // Methods
        public override void Execute(string argument) {
            var arrayLength = SymbolTable.instance[arraysymbolName].ArrayLength;

            Executer.instance.ExecuteInmediately(argument: $"{arrayLength}");

            isFinished = true;
        }

        public GetLengthOfArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeController array
        ) : base(parentList, controllerReference) {

            arraysymbolName = array.symbolName;

        }

    }
}