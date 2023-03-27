// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class ScanAndAddToArrayBuilder : InterpreterElementBuilder {

        // Editor variable
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ScanAndAddToArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                array: arrayContainer.First
            );
        }

    }

    class ScanAndAddToArrayInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingInstruction, AddingToArray }

        // State variables
        Steps currentStep;

        string arraySymbolName;

        /// Lazy variables
        public override bool isExpression => false;

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
            var arrayLength = SymbolTable.instance[arraySymbolName].ArrayLength;

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {arraySymbolName} ha alcanzado su límite.",
                    status: MessageStatus.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            SymbolTable.instance[arraySymbolName].AddToArray(value: newValue);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingInstruction;
        }

        public ScanAndAddToArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeController array
        ) : base(parentList, controllerReference) {

            arraySymbolName = array.symbolName;

        }

    }

}