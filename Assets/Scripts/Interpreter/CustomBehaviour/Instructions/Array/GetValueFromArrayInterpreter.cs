// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class GetValueFromArrayInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingIndex, GettingValueFromArray }

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer arrayContainer;

        // State variables
        Steps currentStep;

        // Lazy variables
        public override bool IsExpression => true;

        NodeController array => arrayContainer.First;
        NodeController indexValue => indexContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public override void Execute(string argument) {

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

            if (index < 0) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser mayor o igual a 0.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            if (index >= arrayLength) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser menor o igual a la longitud: {arrayLength} del arreglo: {symbolName}.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            var arrayValue = SymbolTable.instance[symbolName].GetValueFromArray(index: index);
            Executer.instance.ExecuteInmediately(argument: arrayValue);

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingIndex;
        }

    }

}
