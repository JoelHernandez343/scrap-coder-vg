// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class GetValueFromArrayInterpreter : MonoBehaviour, IInterpreterElement {

        // Private types
        enum Steps { PushingIndex, GettingValueFromArray }

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
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

        public bool IsExpression => true;
        public NodeController Controller => ownTransform.controller;

        NodeController array => arrayContainer.First;
        NodeController indexValue => indexContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public void Execute(string argument) {

            if (currentStep == Steps.PushingIndex) {
                PushingIndex();
            } else if (currentStep == Steps.GettingValueFromArray) {
                GettingValueFromArray(indexValue: argument);
            }

        }

        void PushingIndex() {
            Executer.instance.PushNext(indexValue.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.GettingValueFromArray;
        }

        void GettingValueFromArray(string indexValue) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            var index = System.Int32.Parse(indexValue);

            if (index < 0 || index >= arrayLength) {
                Debug.LogError($"{index} is out of bounds");
                Executer.instance.Stop(force: true);
                return;
            }

            var arrayValue = SymbolTable.instance[symbolName].GetValueFromArray(index: index);
            Executer.instance.ExecuteInmediately(argument: arrayValue);

            IsFinished = true;
        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingIndex;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

    }

}
