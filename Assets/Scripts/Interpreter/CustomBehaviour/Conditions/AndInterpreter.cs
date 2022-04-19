// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class AndInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingLeftCondition, EvaluatingLeftCondition, PushingRightCondition, EvaluatingRightCondition }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

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

        // Methods
        public void Execute(string answer) {

            if (currentStep == Steps.PushingLeftCondition) {
                PushingCondition("left");
            } else if (currentStep == Steps.EvaluatingLeftCondition) {
                EvaluationCondition(answer, "left");
            } else if (currentStep == Steps.PushingRightCondition) {
                PushingCondition("right");
            } else if (currentStep == Steps.EvaluatingRightCondition) {
                EvaluationCondition(answer, "right");
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingLeftCondition;
        }

        public IInterpreterElement GetNextStatement() => null;

        void PushingCondition(string condition) {
            Debug.Log($"Pushing {condition} condition");

            var conditionToPush = condition == "left"
                ? leftCondition
                : rightCondition;

            Executer.instance.PushNext(conditionToPush.interpreterElement);
            Executer.instance.ExecuteInNextFrame();

            currentStep = condition == "left"
                ? Steps.EvaluatingLeftCondition
                : Steps.EvaluatingRightCondition;
        }

        void EvaluationCondition(string value, string condition) {
            Debug.Log($"Evaluating {condition} condition result: {value}");

            if (value == "true") {
                currentStep = Steps.PushingRightCondition;

                if (condition == "left") {
                    Executer.instance.ExecuteInNextFrame();
                } else {
                    IsFinished = true;
                    Executer.instance.ExecuteInNextFrame("true");
                }
            } else {
                IsFinished = true;
                Executer.instance.ExecuteInNextFrame("false");
            }
        }

    }
}