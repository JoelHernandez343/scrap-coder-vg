// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class RepeatNBuilder : InterpreterElementBuilder {

        // Internal types
        enum Steps { PushingValue, EvaluatingValue, ExecutingInstructions }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer instructionsContainer;

        // State variables
        Steps currentStep = Steps.PushingValue;
        int repetition = 0;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController value => variableContainer.array.First;
        NodeController firstInstruction => instructionsContainer.array.First;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.EvaluatingValue) {
                EvaluatingValue(value: argument);
            } else if (currentStep == Steps.ExecutingInstructions) {
                ExecutingInstructions();
            }

        }

        protected override void CustomReset() {
            currentStep = Steps.PushingValue;
            repetition = 0;
        }

        void PushingValue() {
            Executer.instance.PushNext(next: value.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.EvaluatingValue;
        }

        void EvaluatingValue(string value) {
            var number = System.Int32.Parse(value);

            if (repetition < number) {
                repetition += 1;
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

            currentStep = Steps.PushingValue;
        }

    }

}