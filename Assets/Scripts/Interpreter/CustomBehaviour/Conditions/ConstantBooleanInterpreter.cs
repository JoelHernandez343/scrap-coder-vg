// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ConstantBooleanInterpreter : InterpreterElement {

        // Internal types
        enum TypeOfBoolean { True, False }

        // Editor variables
        [SerializeField] TypeOfBoolean typeOfBoolean;

        // Lazy variables
        public override bool IsExpression => true;

        // Methods
        public override void Execute(string argument) {
            Executer.instance.ExecuteInmediately(
                argument: typeOfBoolean == TypeOfBoolean.True ? "true" : "false"
            );

            IsFinished = true;
        }

        public override InterpreterElement GetNextStatement() => null;
    }
}
