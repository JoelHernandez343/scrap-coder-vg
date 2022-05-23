// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class BeginBuilder : InterpreterElementBuilder {

        // Lazy variables
        public override bool IsExpression => false;

        // Methods
        public override void Execute(string argument) {
            Executer.instance.ExecuteInmediately();
            IsFinished = true;
        }

        public override InterpreterElementBuilder GetNextStatement() {
            return Controller.siblings[0].interpreterElement;
        }
    }
}