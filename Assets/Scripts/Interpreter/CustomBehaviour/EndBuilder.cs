// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class EndBuilder : InterpreterElementBuilder {

        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new EndInterpreter(
                parentList: null,
                controllerReference: Controller
            );
        }

    }

    public class EndInterpreter : InterpreterElement {

        // Lazy variables
        public override bool isExpression => false;
        public override bool isFinished => true;

        // Methods
        public override void Execute(string argument) { }

        public override InterpreterElement NextStatement() => null;

        public EndInterpreter(List<InterpreterElement> parentList, NodeController controllerReference)
            : base(parentList, controllerReference) { }

    }
}