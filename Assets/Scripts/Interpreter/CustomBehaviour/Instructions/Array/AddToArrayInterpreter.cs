// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class AddToArrayInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingValue, AddingToArray }

        // Editor variables
        [SerializeField] NodeContainer valueContainer;
        [SerializeField] NodeContainer arrayContainer;

        // State variables
        Steps currentStep;

        // Lazy variables

        public override bool IsExpression => false;

        NodeController array => arrayContainer.First;
        NodeController value => valueContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public override void Execute(string argument) {

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

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {symbolName} ha alcanzado su limite de {Symbol.ArrayLimit}",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            SymbolTable.instance[symbolName].AddToArray(value: value);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingValue;
        }

    }

}