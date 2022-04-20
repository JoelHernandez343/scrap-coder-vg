// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class InteractInterpreter : MonoBehaviour, IInterpreterElement {

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
        public void Execute(string answer) {

            Debug.Log($"Interacting");

            SendInstruction.sendInstruction((int)Actions.Interact);
            // Executer.instance.ExecuteInNextFrame();

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