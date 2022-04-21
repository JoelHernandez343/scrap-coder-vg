// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class NumericValueInterpreter : MonoBehaviour, IInterpreterElement {

        // Editor variables
        [SerializeField] InputText inputText;

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
            var value = inputText.Value;

            Executer.instance.ExecuteInNextFrame(value);

            IsFinished = true;
        }

        public IInterpreterElement GetNextStatement() => null;

        public void Reset() {
            IsFinished = false;
        }
    }
}