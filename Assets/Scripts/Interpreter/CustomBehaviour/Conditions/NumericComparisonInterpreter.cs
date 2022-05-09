// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class NumericComparisonInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingLeftValue, PushingRightValue, EvaluatingCondition }
        enum Comparison { IsEqual, IsNotEqual, IsLessThan, IsLessOrEqual, IsGreaterThan, IsGreaterOrEqual }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] DropMenuController dropMenu;

        // State variables
        Steps currentStep;

        int leftNumber;
        int rightNumber;

        // Lazy variables
        public override bool IsExpression => true;

        NodeController leftValue => leftContainer.First;
        NodeController rightValue => rightContainer.First;

        Comparison comparisonSelected
            => dropMenu.Value == "0"
                ? Comparison.IsEqual
                : dropMenu.Value == "1"
                ? Comparison.IsNotEqual
                : dropMenu.Value == "2"
                ? Comparison.IsLessThan
                : dropMenu.Value == "3"
                ? Comparison.IsLessOrEqual
                : dropMenu.Value == "4"
                ? Comparison.IsGreaterThan
                : Comparison.IsGreaterOrEqual;


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

        protected override void CustomReset() {
            currentStep = Steps.PushingLeftValue;
        }

        public override InterpreterElement GetNextStatement() => null;

        void PushingValue(string member) {

            var valueToPush = member == "left" ? leftValue : rightValue;

            Executer.instance.PushNext(valueToPush.interpreterElement);
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

            if (comparison == Comparison.IsEqual) {
                condition = leftNumber == rightNumber;
            } else if (comparison == Comparison.IsNotEqual) {
                condition = leftNumber != rightNumber;
            } else if (comparison == Comparison.IsLessThan) {
                condition = leftNumber < rightNumber;
            } else if (comparison == Comparison.IsGreaterThan) {
                condition = leftNumber > rightNumber;
            } else if (comparison == Comparison.IsLessOrEqual) {
                condition = leftNumber <= rightNumber;
            } else if (comparison == Comparison.IsGreaterOrEqual) {
                condition = leftNumber >= rightNumber;
            }

            Executer.instance.ExecuteInmediately(argument: condition ? "true" : "false");
            IsFinished = true;

        }

    }

}
