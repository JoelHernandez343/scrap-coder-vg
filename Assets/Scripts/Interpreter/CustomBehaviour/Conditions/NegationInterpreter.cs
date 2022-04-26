// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class NegationInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition }

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;

        // State variables
        Steps currentStep = Steps.PushingCondition;

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

        NodeController condition => conditionContainer.array.First;

        // Methods
        public void Execute(string argument) {

            if (currentStep == Steps.PushingCondition) {
                PushingCondition();
            } else if (currentStep == Steps.EvaluatingCondition) {
                EvaluationCondition(conditionValue: argument);
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingCondition;
        }

        public IInterpreterElement GetNextStatement() => null;

        void PushingCondition() {
            // Debug.Log("Pushing condition");

            Executer.instance.PushNext(condition.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.EvaluatingCondition;
        }

        void EvaluationCondition(string conditionValue) {
            // Debug.Log($"Evaluating condition result: {value}");

            Executer.instance.ExecuteInmediately(argument: conditionValue == "true" ? "false" : "true");

            IsFinished = true;
        }

    }
}