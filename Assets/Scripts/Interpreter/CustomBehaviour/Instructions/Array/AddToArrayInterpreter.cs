// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class AddToArrayInterpreter : MonoBehaviour, IInterpreterElement {

        // Private types
        enum Steps { PushingValue, AddingToArray }

        // Editor variables
        [SerializeField] NodeContainer valueContainer;
        [SerializeField] NodeContainer arrayContainer;

        // State variables
        Steps currentStep;

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

        NodeController array => arrayContainer.First;
        NodeController value => valueContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.AddingToArray) {
                AddingToArray(value: argument);
            }

        }

        void PushingValue() {
            Executer.instance.PushNext(value.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.AddingToArray;
        }

        void AddingToArray(string value) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            if (arrayLength == 100) {
                Debug.LogError("Array has reached its limit.");
                Executer.instance.Stop(force: true);
                return;
            }

            SymbolTable.instance[symbolName].AddToArray(value: value);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingValue;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

    }

}