// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class RotateBuilder : InterpreterElementBuilder {

        // Editor variablesRotateInterpreter
        [SerializeField] DropMenuController dropMenu;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new RotateInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                dropMenuValue: dropMenu.Value
            );
        }

    }

    class RotateInterpreter : InterpreterElement {

        // State variable
        string dropMenuValue;

        /// Lazy variables
        public override bool isExpression => false;

        // Methods
        public override void Execute(string argument) {
            var selectedAction = dropMenuValue == "right" ? Actions.RotateRight : Actions.RotateLeft;
            SendInstruction.sendInstruction((int)selectedAction);
            isFinished = true;
        }

        public RotateInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            string dropMenuValue
        ) : base(parentList, controllerReference) {

            this.dropMenuValue = dropMenuValue;

        }

    }

}
