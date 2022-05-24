// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class InteractBuilder : InterpreterElementBuilder {

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new InteractInterpreter(
                parentList: parentList,
                controllerReference: Controller
            );
        }

    }

    class InteractInterpreter : InterpreterElement {

        /// Lazy variables
        public override bool isExpression => false;

        // Methods
        public override void Execute(string answer) {
            SendInstruction.sendInstruction((int)Actions.Interact);
            isFinished = true;
        }

        public InteractInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference
        ) : base(parentList, controllerReference) { }

    }

}