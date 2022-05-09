// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class EndInterpreter : InterpreterElement {

        // Lazy variables
        public override bool IsFinished => true;

        public override bool IsExpression => false;

        // Methods
        public override void Execute(string argument) { }

        public override InterpreterElement GetNextStatement() => null;

    }
}