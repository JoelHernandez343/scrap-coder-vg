// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class InsertInArrayInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingValue, PushingIndex, InsertingToArray }

        // Editor variables
        [SerializeField] NodeContainer valueContainer;
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer arrayContainer;

        // State variables
        Steps currentStep;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController array => arrayContainer.First;
        NodeController valueToAdd => valueContainer.First;
        NodeController indexValue => indexContainer.First;

        string symbolName => array.symbolName;

        string valueObtained;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                Pushing(which: "value");
            } else if (currentStep == Steps.PushingIndex) {
                StoreValue(value: argument);
                Pushing(which: "index");
            } else if (currentStep == Steps.InsertingToArray) {
                InsertingToArray(indexValue: argument);
            }

        }

        void StoreValue(string value) {
            Debug.Log(value);
            valueObtained = value;
        }

        void Pushing(string which) {
            var elementToPush = which == "value"
                ? valueToAdd.interpreterElement
                : indexValue.interpreterElement;

            Executer.instance.PushNext(elementToPush);
            Executer.instance.ExecuteInmediately();

            currentStep = which == "value"
                ? Steps.PushingIndex
                : Steps.InsertingToArray;
        }

        void InsertingToArray(string indexValue) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            var index = System.Int32.Parse(indexValue);

            if (index < 0 || index >= 100) {
                Debug.LogError($"{index} is out of bounds");
                Executer.instance.Stop(force: true);
                return;
            }

            if (arrayLength == 100) {
                Debug.LogError($"Array has reached its limit");
                Executer.instance.Stop(force: true);
                return;
            }

            SymbolTable.instance[symbolName].InsertToArray(index: index, value: valueObtained);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingValue;
        }

    }

}