// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class GetValueFromArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new GetValueFromArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                indexContainer: indexContainer,
                array: arrayContainer.First
            );
        }

    }

    class GetValueFromArrayInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingIndex, GettingValueFromArray }

        // State variables
        Steps currentStep;

        List<InterpreterElement> indexList = new List<InterpreterElement>();

        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => true;

        InterpreterElement indexValue => indexList[0];

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingIndex) {
                PushingIndex();
            } else if (currentStep == Steps.GettingValueFromArray) {
                GettingValueFromArray(indexValue: argument);
            }

        }

        void PushingIndex() {
            Executer.instance.PushNext(indexValue);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.GettingValueFromArray;
        }

        void GettingValueFromArray(string indexValue) {
            var arrayLength = SymbolTable.instance[arraySymbolName].ArrayLength;

            var index = System.Int32.Parse(indexValue);

            if (index < 0) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser mayor o igual a 0.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            if (index >= arrayLength) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} debe de ser menor o igual a la longitud: {arrayLength} del arreglo: {arraySymbolName}.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            var arrayValue = SymbolTable.instance[arraySymbolName].GetValueFromArray(index: index);
            Executer.instance.ExecuteInmediately(argument: arrayValue);

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingIndex;
        }

        public GetValueFromArrayInterpreter(
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
