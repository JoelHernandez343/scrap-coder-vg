// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class RepeatInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition, ExecutingInstructions }

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;
        [SerializeField] NodeContainer instructionsContainer;

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

        public bool IsExpression => false;
        public NodeController Controller => ownTransform.controller;

        NodeController condition => conditionContainer.array.First;
        NodeController firstInstruction => instructionsContainer.array.First;

        // Methods
        public void Execute(string argument) {

            if (currentStep == Steps.PushingCondition) {
                PushingCondition();
            } else if (currentStep == Steps.EvaluatingCondition) {
                EvaluationCondition(conditionValue: argument);
            } else if (currentStep == Steps.ExecutingInstructions) {
                ExecutingInstructions();
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingCondition;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

        void PushingCondition() {
            // Debug.Log("Pushing condition");

            Executer.instance.PushNext(condition.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.EvaluatingCondition;
        }

        void EvaluationCondition(string conditionValue) {
            // Debug.Log($"Evaluating condition result: {value}");

            if (conditionValue == "true") {
                currentStep = Steps.ExecutingInstructions;
            } else {
                IsFinished = true;
            }

            Executer.instance.ExecuteInmediately();
        }

        void ExecutingInstructions() {
            // Debug.Log("Executing instructions");

            Executer.instance.PushNext(firstInstruction.interpreterElement);
            Executer.instance.ExecuteInNextFrame();

            currentStep = Steps.PushingCondition;
        }

    }

}