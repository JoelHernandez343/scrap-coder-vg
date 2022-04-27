// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class GetLengthOfArrayInterpreter : MonoBehaviour, IInterpreterElement {

        // Editor variables
        [SerializeField] NodeContainer arrayContainer;

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

        NodeController array => arrayContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public void Execute(string argument) {

            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            Executer.instance.ExecuteInmediately(argument: $"{arrayLength}");

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