// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {


    public class ScanAndSetValueOfArrayBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer indexContainer;
        [SerializeField] NodeContainer arrayContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ScanAndSetValueOfArrayInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                indexContainer: indexContainer,
                array: arrayContainer.First
            );
        }

    }

    class ScanAndSetValueOfArrayInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingInstruction, PushingIndex, ReplacingInArray }

        // State variables
        Steps currentStep;

        List<InterpreterElement> indexList = new List<InterpreterElement>();
        string arraySymbolName;

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement indexValue => indexList[0];

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
                Executer.instance.PushNext(indexValue);
                Executer.instance.ExecuteInmediately();

                currentStep = Steps.ReplacingInArray;
            }

        }

        void ReplacingInArray(string indexValue) {
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

            if (index >= Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El índice {index} para insertar no debe sobrepasar el límite {Symbol.ArrayLimit}.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            if (arrayLength == Symbol.ArrayLimit) {
                MessagesController.instance.AddMessage(
                    message: $"El arreglo {arraySymbolName} ha alcanzado su límite.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            SymbolTable.instance[arraySymbolName].SetValueInArray(index: index, newValue: valueObtained);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingInstruction;
        }

        public ScanAndSetValueOfArrayInterpreter(
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



