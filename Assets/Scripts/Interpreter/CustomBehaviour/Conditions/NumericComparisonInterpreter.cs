// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class NumericComparisonInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingLeftValue, PushingRightValue, EvaluatingCondition }
        enum Comparison { IsEqual, IsDifferent, IsLessThan, IsGreaterThan, IsLessOrEqual, IsGreaterOrEqual }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] Comparison comparison;

        // State variables
        Steps currentStep;

        int leftNumber;
        int rightNumber;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool _isFinished = false;
        public bool IsFinished {
            get => _isFinished;
            set => _isFinished = value;
        }

        public bool IsExpression => true;
        public NodeController Controller => ownTransform.controller;

        NodeController leftValue => leftContainer.First;
        NodeController rightValue => rightContainer.First;

        // Methods
        public void Execute(string argument) {

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

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingLeftValue;
        }

        public IInterpreterElement GetNextStatement() => null;

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

            if (comparison == Comparison.IsEqual) {
                condition = leftNumber == rightNumber;
            } else if (comparison == Comparison.IsDifferent) {
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
