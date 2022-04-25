// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ModifyVariableInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Operation { Add, Decrease }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] Operation operation;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool _isFinished = false;
        public bool IsFinished {
            get => _isFinished;
            set => _isFinished = value;
        }

        public bool IsExpression => false;
        public NodeController Controller => ownTransform.controller;

        NodeController variable => variableContainer.array.First;
        string symbolName => variable.symbolName;

        int variableValue {
            get => System.Int32.Parse(SymbolTable.instance[symbolName].value);
            set => SymbolTable.instance[symbolName].value = $"{value}";
        }

        public void Execute(string answer) {

            if (operation == Operation.Add) {
                variableValue = variableValue + 1;
            } else if (operation == Operation.Decrease) {
                variableValue = variableValue - 1;
            }

            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

        public void Reset() {
            IsFinished = false;
        }
    }
}