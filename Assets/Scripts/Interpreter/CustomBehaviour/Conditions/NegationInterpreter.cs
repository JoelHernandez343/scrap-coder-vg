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
        public void Execute(string answer) {

            if (currentStep == Steps.PushingCondition) {
                PushingCondition();
            } else if (currentStep == Steps.EvaluatingCondition) {
                EvaluationCondition(answer);
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingCondition;
        }

        public IInterpreterElement GetNextStatement() => null;

        void PushingCondition() {
            Debug.Log("Pushing condition");

            Executer.instance.PushNext(condition.interpreterElement);
            Executer.instance.ExecuteInNextFrame();

            currentStep = Steps.EvaluatingCondition;
        }

        void EvaluationCondition(string value) {
            Debug.Log($"Evaluating condition result: {value}");

            Executer.instance.ExecuteInNextFrame(value == "true" ? "false" : "true");

            IsFinished = true;
        }

    }
}