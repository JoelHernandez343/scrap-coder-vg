// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class ScanAndAddToArrayBuilder : InterpreterElementBuilder {

        // Internal types
        enum Steps { PushingInstruction, AddingToArray }

        // Editor variable
        [SerializeField] NodeContainer arrayContainer;

        // State variables
        Steps currentStep;

        /// Lazy variables
        public override bool IsExpression => false;

        NodeController array => arrayContainer.First;
        string symbolName => array.symbolName;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingInstruction) {
                PushingInstruction();
            } else {
                AddingToArray(newValue: argument);
            }
        }

        void PushingInstruction() {
            SendInstruction.sendInstruction((int)Actions.Scan);
            currentStep = Steps.AddingToArray;
        }

        void AddingToArray(string newValue) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {symbolName} ha alcanzado su límite.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(force: true);
                return;
            }

            SymbolTable.instance[symbolName].AddToArray(value: newValue);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingInstruction;
        }

    }

}