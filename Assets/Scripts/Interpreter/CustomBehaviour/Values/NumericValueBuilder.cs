// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class NumericValueBuilder : InterpreterElementBuilder, INodeControllerInitializer {

        // Editor variables
        [SerializeField] InputText inputText;

        // State variables
        bool initialized = false;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new NumericValueInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                inputValue: inputText.Value
            );
        }

        Dictionary<string, object> INodeControllerInitializer.GetCustomInfo()
            => new Dictionary<string, object> {
                ["inputText"] = inputText.Value
            };

        void INodeControllerInitializer.Initialize(Dictionary<string, object> customInfo) {
            if (initialized) return;
            if (customInfo == null) return;

            inputText.ChangeValue(
                newText: customInfo["inputText"] as string,
                smooth: false
            );

            initialized = true;
        }

    }

    class NumericValueInterpreter : InterpreterElement {

        // State variables
        string inputValue;

        // Lazy variables
        public override bool isExpression => true;

        public override void Execute(string argument) {
            Executer.instance.ExecuteInmediately(argument: inputValue);
            isFinished = true;
        }

        public override InterpreterElement NextStatement() => null;

        public NumericValueInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            string inputValue
        ) : base(parentList, controllerReference) {
            this.inputValue = inputValue;
        }

    }
}