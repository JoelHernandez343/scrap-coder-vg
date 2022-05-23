// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class IfElseBuilder : InterpreterElementBuilder {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition, ExecutingFirst, ExecutingSecond }

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;
        [SerializeField] NodeContainer firstInstructionsContainer;
        [SerializeField] NodeContainer secondInstructionsContainer;

        // State variables
        Steps currentStep = Steps.PushingCondition;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController condition => conditionContainer.array.First;
        NodeController firstInstruction => firstInstructionsContainer.array.First;
        NodeController secondInstruction => secondInstructionsContainer.array.First;

        // Methods
        public override void Execute(string argument) {

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