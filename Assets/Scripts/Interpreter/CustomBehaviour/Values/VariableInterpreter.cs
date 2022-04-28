// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class VariableInterpreter : InterpreterElement {

        // Lazy variables
        public override bool IsExpression => true;

        public override void Execute(string argument) {
            var value = SymbolTable.instance[Controller.symbolName].value;

            Executer.instance.ExecuteInmediately(argument: value);

            IsFinished = true;
        }

        public override InterpreterElement GetNextStatement() => null;

    }
}