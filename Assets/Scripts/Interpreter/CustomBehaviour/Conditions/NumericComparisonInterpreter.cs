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
        public void Execute(string answer) {

            if (currentStep == Steps.PushingLeftValue) {
                PushingValue("left");
            } else if (currentStep == Steps.PushingRightValue) {
                StoreValue("left", answer);
                PushingValue("right");
            } else if (currentStep == Steps.EvaluatingCondition) {
                StoreValue("right", answer);
                EvaluationCondition();
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingLeftValue;
        }

        public IInterpreterElement GetNextStatement() => null;

        void PushingValue(string condition) {

            var valueToPush = condition == "left" ? leftValue : rightValue;

            Executer.instance.PushNext(valueToPush.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = condition == "left"
                ? Steps.PushingRightValue
                : Steps.EvaluatingCondition;
        }

        void StoreValue(string which, string value) {
            if (which == "left") {
                leftNumber = System.Int32.Parse(value);
            } else {
                rightNumber = System.Int32.Parse(value);
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

            Executer.instance.ExecuteInmediately(condition ? "true" : "false");
            IsFinished = true;

        }
    }

}
