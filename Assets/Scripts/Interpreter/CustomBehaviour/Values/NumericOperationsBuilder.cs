// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class NumericOperationsBuilder : InterpreterElementBuilder, INodeControllerInitializer {

        // Internal types
        public enum Operation { Sum, Substraction, Multiplication, Division }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] DropMenuController dropMenu;

        // State variables
        bool initialized = false;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new NumericOperationsInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                leftValueContainer: leftContainer,
                rightValueContainer: rightContainer,
                dropMenuValue: dropMenu.Value
            );
        }

        Dictionary<string, object> INodeControllerInitializer.GetCustomInfo()
            => new Dictionary<string, object> {
                ["selectedOptionText"] = dropMenu.selectedOption.text,
                ["selectedOptionValue"] = dropMenu.selectedOption.value,
            };

        void INodeControllerInitializer.Initialize(Dictionary<string, object> customInfo) {
            if (initialized) return;
            if (customInfo == null) return;

            dropMenu.ChangeOption(
                newOption: new DropMenuOption {
                    text = customInfo["selectedOptionText"] as string,
                    value = customInfo["selectedOptionValue"] as string
                },
                executeListeners: true
            );

            initialized = true;
        }

    }

    class NumericOperationsInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingLeftValue, PushingRightValue, EvaluatingCondition }

        // State variables
        Steps currentStep;

        string dropMenuValue;

        int leftNumber;
        int rightNumber;

        List<InterpreterElement> leftValueList = new List<InterpreterElement>();
        List<InterpreterElement> rightValueList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => true;

        InterpreterElement leftValue => leftValueList[0];
        InterpreterElement rightValue => rightValueList[0];

        NumericOperationsBuilder.Operation selectedOperation
            => dropMenuValue == "a"
                ? NumericOperationsBuilder.Operation.Sum
                : dropMenuValue == "s"
                ? NumericOperationsBuilder.Operation.Substraction
                : dropMenuValue == "m"
                ? NumericOperationsBuilder.Operation.Multiplication
                : NumericOperationsBuilder.Operation.Division;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingLeftValue) {
                PushingValue(member: "left");
            } else if (currentStep == Steps.PushingRightValue) {
                StoreValue(member: "left", numericValue: argument);
                PushingValue("right");
            } else if (currentStep == Steps.EvaluatingCondition) {
                StoreValue(member: "right", numericValue: argument);
                EvaluationCondition();
            }

        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingLeftValue;
        }

        public override InterpreterElement NextStatement() => null;

        void PushingValue(string member) {

            var valueToPush = member == "left" ? leftValue : rightValue;

            Executer.instance.PushNext(valueToPush);
            Executer.instance.ExecuteInmediately();

            currentStep = member == "left"
                ? Steps.PushingRightValue
                : Steps.EvaluatingCondition;
        }

        void StoreValue(string member, string numericValue) {
            if (member == "left") {
                leftNumber = System.Int32.Parse(numericValue);
            } else {
                rightNumber = System.Int32.Parse(numericValue);
            }
        }

        void EvaluationCondition() {

            var result = 0;
            var operation = selectedOperation;

            if (operation == NumericOperationsBuilder.Operation.Sum) {
                result = leftNumber + rightNumber;
            } else if (operation == NumericOperationsBuilder.Operation.Substraction) {
                result = leftNumber - rightNumber;
            } else if (operation == NumericOperationsBuilder.Operation.Multiplication) {
                result = leftNumber * rightNumber;
            } else if (operation == NumericOperationsBuilder.Operation.Division) {
                if (rightNumber == 0) {
                    MessagesController.instance.AddMessage(
                        message: $"División entre 0 detectada en la op: {leftNumber} / {rightNumber}.",
                        status: MessageStatus.Error
                    );
                    Executer.instance.Stop(successfully: false);
                    return;
                }

                result = leftNumber / rightNumber;
            }

            Executer.instance.ExecuteInmediately(argument: $"{result}");
            isFinished = true;

        }

        public NumericOperationsInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer leftValueContainer,
            NodeContainer rightValueContainer,
            string dropMenuValue
        ) : base(parentList, controllerReference) {

            leftValueList.AddRange(InterpreterElementsFromContainer(
                container: leftValueContainer,
                parentList: leftValueList
            ));

            rightValueList.AddRange(InterpreterElementsFromContainer(
                container: rightValueContainer,
                parentList: rightValueList
            ));

            this.dropMenuValue = dropMenuValue;

        }

    }

}
