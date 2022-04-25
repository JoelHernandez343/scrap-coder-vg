// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class VariableInterpreter : MonoBehaviour, IInterpreterElement {

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool _isFinished = false;
        public bool IsFinished {
            get => _isFinished;
            set => _isFinished = value;
        }

        public bool IsExpression => true;

        public NodeController Controller => ownTransform.controller;

        public void Execute(string answer) {
            var value = SymbolTable.instance[Controller.symbolName].value;

            Executer.instance.ExecuteInmediately(answer: value);

            IsFinished = true;
        }

        public IInterpreterElement GetNextStatement() => null;

        public void Reset() {
            IsFinished = false;
        }
    }
}