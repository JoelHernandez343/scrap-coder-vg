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

        bool leftValue;
        bool rightValue;

        // Methods
        public void Execute(string answer) {

            if (currentStep == Steps.PushingLeftCondition) {
                PushingCondition("left");
            } else if (currentStep == Steps.EvaluatingLeftCondition) {
                StoreValue("left", answer);
                EvaluationCondition(answer, "left");
            } else if (currentStep == Steps.PushingRightCondition) {
                PushingCondition("right");
            } else if (currentStep == Steps.EvaluatingRightCondition) {
                StoreValue("right", answer);
                EvaluationCondition(answer, "right");
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingLeftCondition;
        }

        public IInterpreterElement GetNextStatement() => null;

        void PushingCondition(string condition) {
            // Debug.Log($"Pushing {condition} condition");

            var conditionToPush = condition == "left"
                ? leftCondition
                : rightCondition;

            Executer.instance.PushNext(conditionToPush.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = condition == "left"
                ? Steps.EvaluatingLeftCondition
                : Steps.EvaluatingRightCondition;
        }

        void EvaluationCondition(string value, string condition) {

            if (operation == Operation.Or || operation == Operation.And) {
                var finisher = operation == Operation.Or ? "true" : "false";
                var opposite = finisher == "true" ? "false" : "true";

                if (value == finisher) {
                    IsFinished = true;
                    Executer.instance.ExecuteInmediately(answer: finisher);
                } else {
                    currentStep = Steps.PushingRightCondition;

                    if (condition == "left") {
                        Executer.instance.ExecuteInmediately();
                    } else {
                        IsFinished = true;
                        Executer.instance.ExecuteInmediately(answer: opposite);
                    }
                }
            }

        }

        void StoreValue(string which, string value) {
            if (which == "left") {
                leftValue = value == "true";
            } else {
                rightValue = value == "true";
            }
        }

    }
}
