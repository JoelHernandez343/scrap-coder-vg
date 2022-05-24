// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class WalkBuilder : InterpreterElementBuilder {

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new WalkInterpreter(
                parentList: parentList,
                controllerReference: Controller
            );
        }

    }

    class WalkInterpreter : InterpreterElement {

        // Lazy variables
        public override bool isExpression => false;

        // Methods
        public override void Execute(string argument) {
            SendInstruction.sendInstruction((int)Actions.Walk);
            isFinished = true;
        }

        public WalkInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference
        ) : base(parentList, controllerReference) { }

    }

}