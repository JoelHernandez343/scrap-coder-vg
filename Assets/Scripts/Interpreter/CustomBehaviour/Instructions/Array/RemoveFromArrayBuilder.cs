// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class RemoveFromArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new RemoveFromArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                indexContainer: indexContainer,
                array: arrayContainer.First
            );
        }

    }

    class RemoveFromArrayInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingIndex, RemovingFromArray }

        // State variables
        Steps currentStep;

        List<InterpreterElement> indexList = new List<InterpreterElement>();
        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement indexValue => indexList[0];


        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingIndex) {
                PushingIndex();
            } else if (currentStep == Steps.RemovingFromArray) {
                RemovingFromArray(indexValue: argument);
            }

        }

        void PushingIndex() {
            Executer.instance.PushNext(indexValue);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.RemovingFromArray;
        }

        void RemovingFromArray(string indexValue) {
            var arrayLength = SymbolTable.instance[arraySymbolName].ArrayLength;

            var index = System.Int32.Parse(indexValue);

            if (index < 0) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser mayor o igual a 0.",
                    status: MessageStatus.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            if (index >= arrayLength) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser menor o igual a la longitud: {arrayLength} del arreglo: {arraySymbolName}.",
                    status: MessageStatus.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            SymbolTable.instance[arraySymbolName].RemoveFromArray(index: index);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingIndex;
        }

        public RemoveFromArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer indexContainer,
            NodeController array
        ) : base(parentList, controllerReference) {

            indexList.AddRange(InterpreterElementsFromContainer(
                container: indexContainer,
                parentList: indexList
            ));

            arraySymbolName = array.symbolName;

        }

    }

}
