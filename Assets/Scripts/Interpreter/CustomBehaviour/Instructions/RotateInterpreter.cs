// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class RotateInterpreter : MonoBehaviour, IInterpreterElement {

        // Editor variables
        [SerializeField] DropMenuController dropMenu;

        /// Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool _isFinished = false;
        public bool IsFinished {
            get => _isFinished;
            set => _isFinished = value;
        }

        public bool IsExpression => false;
        public NodeController Controller => ownTransform.controller;

        // Methods
        public void Execute(string argument) {

            var selectedAction = dropMenu.Value == "right" ? Actions.RotateRight : Actions.RotateLeft;

            SendInstruction.sendInstruction((int)selectedAction);

            IsFinished = true;
        }

        public void Reset() {
            IsFinished = false;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }
    }

}
