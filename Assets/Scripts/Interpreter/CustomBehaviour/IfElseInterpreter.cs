// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class IfElseInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition, ExecutingFirst, ExecutingSecond }

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;
        [SerializeField] NodeContainer firstInstructionsContainer;
        [SerializeField] NodeContainer secondInstructionsContainer;

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

        NodeController firstInstruction => firstInstructionsContainer.array.First;
        NodeController secondInstruction => secondInstructionsContainer.array.First;

        // Methods
        public void Execute(string argument) {

            if (currentStep == Steps.PushingCondition) {
                PushingCondition();
            } else if (currentStep == Steps.EvaluatingCondition) {
                EvaluationCondition(conditionValue: argument);
            } else if (currentStep == Steps.ExecutingFirst) {
                ExecutingInstructions("first");
            } else if (currentStep == Steps.ExecutingSecond) {
                ExecutingInstructions("second");
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
                currentStep = Steps.ExecutingFirst;
            } else {
                currentStep = Steps.ExecutingSecond;
            }

            Executer.instance.ExecuteInmediately();
        }

        void ExecutingInstructions(string instructions) {
            // Debug.Log("Executing instructions");

            if (instructions == "first") {
                Executer.instance.PushNext(firstInstruction.interpreterElement);
            } else {
                Executer.instance.PushNext(secondInstruction.interpreterElement);
            }

            Executer.instance.ExecuteInNextFrame();

            IsFinished = true;
        }

    }

}