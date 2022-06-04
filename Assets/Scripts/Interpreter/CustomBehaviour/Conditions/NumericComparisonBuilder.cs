// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class NumericComparisonBuilder : InterpreterElementBuilder, INodeControllerInitializer {

        // Internal types
        public enum Comparison { IsEqual, IsNotEqual, IsLessThan, IsLessOrEqual, IsGreaterThan, IsGreaterOrEqual }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] DropMenuController dropMenu;

        // State variables
        bool initialized = false;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new NumericComparisonInterpreter(
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

    class NumericComparisonInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingLeftValue, PushingRightValue, EvaluatingCondition }

        // State variables
        Steps currentStep;

        int leftNumber;
        int rightNumber;

        string dropMenuValue;

        List<InterpreterElement> leftValueList = new List<InterpreterElement>();
        List<InterpreterElement> rightValueList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => true;

        InterpreterElement leftValue => leftValueList[0];
        InterpreterElement rightValue => rightValueList[0];

        NumericComparisonBuilder.Comparison comparisonSelected
            => dropMenuValue == "0"
                ? NumericComparisonBuilder.Comparison.IsEqual
                : dropMenuValue == "1"
                ? NumericComparisonBuilder.Comparison.IsNotEqual
                : dropMenuValue == "2"
                ? NumericComparisonBuilder.Comparison.IsLessThan
                : dropMenuValue == "3"
                ? NumericComparisonBuilder.Comparison.IsLessOrEqual
                : dropMenuValue == "4"
                ? NumericComparisonBuilder.Comparison.IsGreaterThan
                : NumericComparisonBuilder.Comparison.IsGreaterOrEqual;


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

            var condition = false;
            var comparison = comparisonSelected;

            if (comparison == NumericComparisonBuilder.Comparison.IsEqual) {
                condition = leftNumber == rightNumber;
            } else if (comparison == NumericComparisonBuilder.Comparison.IsNotEqual) {
                condition = leftNumber != rightNumber;
            } else if (comparison == NumericComparisonBuilder.Comparison.IsLessThan) {
                condition = leftNumber < rightNumber;
            } else if (comparison == NumericComparisonBuilder.Comparison.IsGreaterThan) {
                condition = leftNumber > rightNumber;
            } else if (comparison == NumericComparisonBuilder.Comparison.IsLessOrEqual) {
                condition = leftNumber <= rightNumber;
            } else if (comparison == NumericComparisonBuilder.Comparison.IsGreaterOrEqual) {
                condition = leftNumber >= rightNumber;
            }

            Executer.instance.ExecuteInmediately(argument: condition ? "true" : "false");
            isFinished = true;

        }

        public NumericComparisonInterpreter(
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
