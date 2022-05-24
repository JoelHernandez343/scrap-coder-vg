// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class NumericValueBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] InputText inputText;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new NumericValueInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                inputValue: inputText.Value
            );
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