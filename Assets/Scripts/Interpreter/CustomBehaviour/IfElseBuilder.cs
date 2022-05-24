// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class IfElseBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;
        [SerializeField] NodeContainer firstInstructionsContainer;
        [SerializeField] NodeContainer secondInstructionsContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new IfElseInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                conditionContainer: conditionContainer,
                firstInstructionsContainer: firstInstructionsContainer,
                secondInstructionsContainer: secondInstructionsContainer
            );
        }

    }

    class IfElseInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition, ExecutingFirst, ExecutingSecond }

        // State variables
        Steps currentStep = Steps.PushingCondition;

        List<InterpreterElement> conditionList = new List<InterpreterElement>();
        List<InterpreterElement> firstInstructions = new List<InterpreterElement>();
        List<InterpreterElement> secondInstructions = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement condition => conditionList[0];
        InterpreterElement firstInstruction => firstInstructions[0];
        InterpreterElement secondInstruction => secondInstructions[0];

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

        protected override void CustomResetState() {
            currentStep = Steps.PushingCondition;
        }

        void PushingCondition() {
            // Debug.Log("Pushing condition");

            Executer.instance.PushNext(condition);
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
                Executer.instance.PushNext(firstInstruction);
            } else {
                Executer.instance.PushNext(secondInstruction);
            }

            Executer.instance.ExecuteInNextFrame();

            isFinished = true;
        }

        public IfElseInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer conditionContainer,
            NodeContainer firstInstructionsContainer,
            NodeContainer secondInstructionsContainer
        ) : base(parentList, controllerReference) {

            conditionList.AddRange(InterpreterElementsFromContainer(
                container: conditionContainer,
                parentList: conditionList
            ));

            firstInstructions.AddRange(InterpreterElementsFromContainer(
                container: firstInstructionsContainer,
                parentList: firstInstructions
            ));

            secondInstructions.AddRange(InterpreterElementsFromContainer(
                container: secondInstructionsContainer,
                parentList: secondInstructions
            ));

        }

    }

}