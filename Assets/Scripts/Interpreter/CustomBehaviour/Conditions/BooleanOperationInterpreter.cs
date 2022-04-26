// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class BooleanOperationInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingLeftCondition, EvaluatingLeftCondition, PushingRightCondition, EvaluatingRightCondition }
        enum Operation { Or, And }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] Operation operation;

        // State variables
        Steps currentStep = Steps.PushingLeftCondition;

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

        NodeController leftCondition => leftContainer.array.First;
        NodeController rightCondition => rightContainer.array.First;

        string leftValue;
        string rightValue;

        // Methods
        public void Execute(string argument) {

            if (currentStep == Steps.PushingLeftCondition) {
                PushingCondition("left");
            } else if (currentStep == Steps.EvaluatingLeftCondition) {
                StoreValue(condition: "left", conditionValue: argument);
                EvaluationCondition(condition: "left");
            } else if (currentStep == Steps.PushingRightCondition) {
                PushingCondition("right");
            } else if (currentStep == Steps.EvaluatingRightCondition) {
                StoreValue(condition: "right", conditionValue: argument);
                EvaluationCondition(condition: "right");
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingLeftCondition;
        }

        public IInterpreterElement GetNextStatement() => null;

        void PushingCondition(string condition) {
            var conditionToPush = condition == "left"
                ? leftCondition
                : rightCondition;

            Executer.instance.PushNext(conditionToPush.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = condition == "left"
                ? Steps.EvaluatingLeftCondition
                : Steps.EvaluatingRightCondition;
        }

        void EvaluationCondition(string condition) {

            var conditionValue = condition == "left" ? leftValue : rightValue;

            if (operation == Operation.Or || operation == Operation.And) {
                var finisher = operation == Operation.Or ? "true" : "false";
                var opposite = finisher == "true" ? "false" : "true";

                if (conditionValue == finisher) {
                    IsFinished = true;
                    Executer.instance.ExecuteInmediately(argument: finisher);
                } else {
                    currentStep = Steps.PushingRightCondition;

                    if (condition == "left") {
                        Executer.instance.ExecuteInmediately();
                    } else {
                        IsFinished = true;
                        Executer.instance.ExecuteInmediately(argument: opposite);
                    }
                }
            }

        }

        void StoreValue(string condition, string conditionValue) {
            if (condition == "left") {
                leftValue = conditionValue;
            } else {
                rightValue = conditionValue;
            }
        }

    }
}
