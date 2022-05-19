// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class ReplaceInArrayInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingValue, PushingIndex, ReplacingInArray }

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer valueContainer;
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
            } else if (currentStep == Steps.ReplacingInArray) {
                ReplacingInArray(indexValue: argument);
            }

        }

        void StoreValue(string value) {
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
                : Steps.ReplacingInArray;
        }

        void ReplacingInArray(string indexValue) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            var index = System.Int32.Parse(indexValue);

            if (index < 0) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser mayor o igual a 0.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            if (index >= Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} para insertar no debe sobrepasar el límite {Symbol.ArrayLimit}.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {symbolName} ha alcanzado su límite.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            SymbolTable.instance[symbolName].SetValueInArray(index: index, newValue: valueObtained);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingValue;
        }

    }

}
