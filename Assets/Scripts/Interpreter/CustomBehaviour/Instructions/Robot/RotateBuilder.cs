// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class RotateBuilder : InterpreterElementBuilder, INodeControllerInitializer {

        // Editor variablesRotateInterpreter
        [SerializeField] DropMenuController dropMenu;

        // State variables
        bool initialized = false;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new RotateInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                dropMenuValue: dropMenu.Value
            );
        }

        Dictionary<string, object> INodeControllerInitializer.GetCustomInfo()
            => new Dictionary<string, object> {
                ["selectedOptionText"] = dropMenu.selectedOption.text,
                ["selectedOptionValue"] = dropMenu.selectedOption.value,
            };

        void INodeControllerInitializer.Initialize(Dictionary<string, object> customInfo) {
            if (initialized) return;
            if (customInfo == null) return;

            dropMenu.ChangeOption(
                newOption: new DropMenuOption {
                    text = customInfo["selectedOptionText"] as string,
                    value = customInfo["selectedOptionValue"] as string
                },
                executeListeners: false
            );

            initialized = true;
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
