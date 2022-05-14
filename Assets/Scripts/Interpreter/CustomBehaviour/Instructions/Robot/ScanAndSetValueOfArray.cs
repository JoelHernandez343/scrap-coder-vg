// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {


    public class ScanAndSetValueOfArray : InterpreterElement {

        // Private types
        enum Steps { PushingInstruction, PushingIndex, ReplacingInArray }

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer arrayContainer;

        // State variables
        Steps currentStep;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController array => arrayContainer.First;
        NodeController indexValue => indexContainer.First;

        string symbolName => array.symbolName;

        string valueObtained;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingInstruction) {
                Pushing(which: "instruction");
            } else if (currentStep == Steps.PushingIndex) {
                StoreValue(value: argument);
                Pushing(which: "index");
            } else if (currentStep == Steps.ReplacingInArray) {
                ReplacingInArray(indexValue: argument);
            }

        }

        void StoreValue(string value) {
            valueObtained = value;
        }

        void Pushing(string which) {

            if (which == "instruction") {
                SendInstruction.sendInstruction((int)Actions.Scan);

                currentStep = Steps.PushingIndex;
            } else {
                Executer.instance.PushNext(indexValue.interpreterElement);
                Executer.instance.ExecuteInmediately();

                currentStep = Steps.ReplacingInArray;
            }

        }

        void ReplacingInArray(string indexValue) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            var index = System.Int32.Parse(indexValue);

            if (index < 0 || index >= Symbol.ArrayLimit) {
                Debug.LogError($"{index} is out of bounds");
                Executer.instance.Stop(force: true);
                return;
            }

            if (arrayLength == Symbol.ArrayLimit) {
                Debug.LogError($"Array has reached its limit");
                Executer.instance.Stop(force: true);
                return;
            }

            SymbolTable.instance[symbolName].SetValueInArray(index: index, newValue: valueObtained);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingInstruction;
        }

    }

}



