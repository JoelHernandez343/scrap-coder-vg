// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class NumericValueInterpreter : InterpreterElement {

        // Editor variables
        [SerializeField] InputText inputText;

        // Lazy variables
        public override bool IsExpression => true;

        public override void Execute(string argument) {
            var value = inputText.Value;

            Executer.instance.ExecuteInmediately(argument: value);

            IsFinished = true;
        }

        public override InterpreterElement GetNextStatement() => null;

    }
}