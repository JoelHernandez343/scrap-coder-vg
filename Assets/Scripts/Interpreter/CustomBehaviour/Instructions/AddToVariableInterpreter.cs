// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class AddToVariableInterpreter : MonoBehaviour, IInterpreterElement {

        // Editor variables
        [SerializeField] NodeContainer variableContainer;

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

        // Methods
        public void Execute(string answer) {

            Debug.Log($"Adding to number variable: {variable.symbolName} with value {SymbolTable.instance[variable.symbolName].value}");

            var value = System.Int32.Parse(SymbolTable.instance[variable.symbolName].value);

            SymbolTable.instance[variable.symbolName].value = $"{value + 1}";

            Executer.instance.ExecuteInNextFrame();

            Debug.Log($"Value of {variable.symbolName} is {SymbolTable.instance[variable.symbolName].value}");

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