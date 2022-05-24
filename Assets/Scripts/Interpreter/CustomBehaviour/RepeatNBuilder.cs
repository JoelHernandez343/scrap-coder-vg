// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class RepeatNBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer instructionsContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new RepeatNInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                valueContainer: variableContainer,
                instructionsContainer: instructionsContainer
            );
        }

    }

    class RepeatNInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingValue, EvaluatingValue, ExecutingInstructions }

        // State variables
        Steps currentStep = Steps.PushingValue;
        int repetition = 0;

        List<InterpreterElement> valueList = new List<InterpreterElement>();
        List<InterpreterElement> instructions = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement value => valueList[0];
        InterpreterElement firstInstruction => instructions[0];

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

        protected override void CustomResetState() {
            currentStep = Steps.PushingValue;
            repetition = 0;
        }

        void PushingValue() {
            Executer.instance.PushNext(next: value);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.EvaluatingValue;
        }

        void EvaluatingValue(string value) {
            var number = System.Int32.Parse(value);

            if (repetition < number) {
                repetition += 1;
                currentStep = Steps.ExecutingInstructions;
            } else {
                isFinished = true;
            }

            Executer.instance.ExecuteInmediately();
        }

        void ExecutingInstructions() {
            // Debug.Log("Executing instructions");

            Executer.instance.PushNext(firstInstruction);
            Executer.instance.ExecuteInNextFrame();

            currentStep = Steps.PushingValue;
        }

        public RepeatNInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer valueContainer,
            NodeContainer instructionsContainer
        ) : base(parentList, controllerReference) {

            valueList.AddRange(InterpreterElementsFromContainer(
                container: valueContainer,
                parentList: valueList
            ));

            instructions.AddRange(InterpreterElementsFromContainer(
                container: instructionsContainer,
                parentList: instructions
            ));

        }

    }

}