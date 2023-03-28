// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class AddToArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer valueContainer;
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new AddToArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                valueContainer: valueContainer,
                array: arrayContainer.First
            );
        }

    }

    class AddToArrayInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingValue, AddingToArray }

        // State variables
        Steps currentStep;

        List<InterpreterElement> valueList = new List<InterpreterElement>();

        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement value => valueList[0];

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.AddingToArray) {
                AddingToArray(value: argument);
            }

        }

        void PushingValue() {
            Executer.instance.PushNext(value);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.AddingToArray;
        }

        void AddingToArray(string value) {
            var arrayLength = SymbolTable.instance[arraySymbolName].ArrayLength;

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {arraySymbolName} ha alcanzado su limite de {Symbol.ArrayLimit}",
                    status: MessageStatus.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            SymbolTable.instance[arraySymbolName].AddToArray(value: value);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingValue;
        }

        public AddToArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer valueContainer,
            NodeController array
        ) : base(parentList, controllerReference) {

            valueList.AddRange(InterpreterElementsFromContainer(
                container: valueContainer,
                parentList: valueList
            ));

            arraySymbolName = array.symbolName;

        }

    }

}