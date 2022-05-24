// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class BeginBuilder : InterpreterElementBuilder {

        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new BeginInterpreter(
                parentList: null,
                controllerReference: Controller
            );
        }
    }

    class BeginInterpreter : InterpreterElement {

        // State variables
        List<InterpreterElement> siblings = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        // Methods
        public override void Execute(string argument) {
            Executer.instance.ExecuteInmediately();
            isFinished = true;
        }

        public BeginInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference
        ) : base(parentList, controllerReference) {

            siblings.AddRange(InterpreterElementsFromContainer(
                container: controllerReference.siblings.container,
                parentList: siblings
            ));

        }

        public override InterpreterElement NextStatement() {
            return siblings[0];
        }

    }
}