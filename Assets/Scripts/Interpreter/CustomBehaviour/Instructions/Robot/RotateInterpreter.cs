// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class RotateInterpreter : InterpreterElement {

        // Editor variables
        [SerializeField] DropMenuController dropMenu;

        /// Lazy variables
        public override bool IsExpression => false;

        // Methods
        public override void Execute(string argument) {
            var selectedAction = dropMenu.Value == "right" ? Actions.RotateRight : Actions.RotateLeft;
            SendInstruction.sendInstruction((int)selectedAction);
            IsFinished = true;
        }

    }

}
