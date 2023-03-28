// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class ReplaceInArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer valueContainer;
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ReplaceInArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                valueContainer: valueContainer,
                indexContainer: indexContainer,
                array: arrayContainer.First
            );
        }

    }

    class ReplaceInArrayInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingValue, PushingIndex, ReplacingInArray }

        // State variables
        Steps currentStep;

        List<InterpreterElement> indexList = new List<InterpreterElement>();
        List<InterpreterElement> valueList = new List<InterpreterElement>();

        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement valueToAdd => valueList[0];
        InterpreterElement indexValue => indexList[0];

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
                ? valueToAdd
                : indexValue;

            Executer.instance.PushNext(elementToPush);
            Executer.instance.ExecuteInmediately();

            currentStep = which == "value"
                ? Steps.PushingIndex
                : Steps.ReplacingInArray;
        }

        void ReplacingInArray(string indexValue) {
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

            if (index >= Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} para insertar no debe sobrepasar el límite {Symbol.ArrayLimit}.",
                    status: MessageStatus.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {arraySymbolName} ha alcanzado su límite.",
                    status: MessageStatus.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            SymbolTable.instance[arraySymbolName].SetValueInArray(index: index, newValue: valueObtained);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingValue;
        }

        public ReplaceInArrayInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer valueContainer,
            NodeContainer indexContainer,
            NodeController array
        ) : base(parentList, controllerReference) {

            indexList.AddRange(InterpreterElementsFromContainer(
                container: indexContainer,
                parentList: indexList
            ));

            valueList.AddRange(InterpreterElementsFromContainer(
                container: valueContainer,
                parentList: valueList
            ));

            arraySymbolName = array.symbolName;

        }

    }

}
