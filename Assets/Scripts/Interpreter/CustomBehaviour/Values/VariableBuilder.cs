// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class VariableBuilder : InterpreterElementBuilder {

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new VariableInterpreter(
                parentList: parentList,
                controllerReference: Controller
            );
        }

    }

    class VariableInterpreter : InterpreterElement {

        // Lazy variables
        public override bool isExpression => true;

        public override void Execute(string argument) {
            var value = SymbolTable.instance[symbolName].Value;

            Executer.instance.ExecuteInmediately(argument: value);

            isFinished = true;
        }

        public override InterpreterElement NextStatement() => null;

        public VariableInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference
        ) : base(parentList, controllerReference) { }

    }
}