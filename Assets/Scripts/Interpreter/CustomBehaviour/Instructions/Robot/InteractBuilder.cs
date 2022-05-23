// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class InteractBuilder : InterpreterElementBuilder {

        /// Lazy variables
        public override bool IsExpression => false;

        // Methods
        public override void Execute(string answer) {
            SendInstruction.sendInstruction((int)Actions.Interact);
            IsFinished = true;
        }

    }

}