// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class NumericOperationsInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingLeftValue, PushingRightValue, EvaluatingCondition }
        enum Operation { Sum, Substraction, Multiplication, Division }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] Operation operation;

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

            var result = 0;

            if (operation == Operation.Sum) {
                result = leftNumber + rightNumber;
            } else if (operation == Operation.Substraction) {
                result = leftNumber - rightNumber;
            } else if (operation == Operation.Multiplication) {
                result = leftNumber * rightNumber;
            } else if (operation == Operation.Division) {
                if (rightNumber == 0) {
                    Debug.LogError("Division by 0 detected!");
                    Executer.instance.Stop();
                    return;
                }

                result = leftNumber / rightNumber;
            }

            Executer.instance.ExecuteInmediately(answer: $"{result}");
            IsFinished = true;

        }
    }

}
