// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class PutValueOnPanelBuilder : InterpreterElementBuilder {

        // Editor variable
        [SerializeField] NodeContainer valueContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new PutValueOnPanelInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                valueContainer: valueContainer
            );
        }

    }

    class PutValueOnPanelInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingValue, PushingInstruction }

        // State variables
        Steps currentStep;

        List<InterpreterElement> valueList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement value => valueList[0];

        public override void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else {
                PushingInstruction(value: argument);
            }
        }

        void PushingValue() {
            Executer.instance.PushNext(value);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.PushingInstruction;
        }

        void PushingInstruction(string value) {
            var number = System.Int32.Parse(value);

            if (number < 0 || number > 6) {
                MessagesController.instance.AddMessage(
                    message: $"El valor que se está intentando ingresar al panel no está entre 0 y 6.",
                    type: MessageType.Error
                );
                Executer.instance.Stop(successfully: false);
                return;
            }

            SendInstruction.sendInstruction((int)Actions.Zero + number);
            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingValue;
        }

        public PutValueOnPanelInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer valueContainer
        ) : base(parentList, controllerReference) {

            valueList.AddRange(InterpreterElementsFromContainer(
                container: valueContainer,
                parentList: valueList
            ));

        }


    }
}