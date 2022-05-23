// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class IfBuilder : InterpreterElementBuilder {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition, ExecutingInstructions }

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;
        [SerializeField] NodeContainer instructionsContainer;

        // State variables
        Steps currentStep = Steps.PushingCondition;

        // Lazy variables

        public override bool IsExpression => false;

        NodeController condition => conditionContainer.array.First;
        NodeController firstInstruction => instructionsContainer.array.First;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingCondition) {
                PushingCondition();
            } else if (currentStep == Steps.EvaluatingCondition) {
                EvaluationCondition(conditionValue: argument);
            } else if (currentStep == Steps.ExecutingInstructions) {
                ExecutingInstructions();
            }

        }

        protected override void CustomReset() {
            currentStep = Steps.PushingCondition;
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

            IsFinished = true;
        }

    }

}