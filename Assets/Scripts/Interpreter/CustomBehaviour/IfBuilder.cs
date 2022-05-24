// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class IfBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;
        [SerializeField] NodeContainer instructionsContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new IfInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                conditionContainer: conditionContainer,
                instructionsContainer: instructionsContainer
            );
        }

    }

    class IfInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition, ExecutingInstructions }

        // State variables
        Steps currentStep = Steps.PushingCondition;

        List<InterpreterElement> conditionList = new List<InterpreterElement>();
        List<InterpreterElement> instructions = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement condition => conditionList[0];
        InterpreterElement firstInstruction => instructions[0];

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

            isFinished = true;
        }

        public IfInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer conditionContainer,
            NodeContainer instructionsContainer
        ) : base(parentList, controllerReference) {

            conditionList.AddRange(InterpreterElementsFromContainer(
                container: conditionContainer,
                parentList: conditionList
            ));

            instructions.AddRange(InterpreterElementsFromContainer(
                container: instructionsContainer,
                parentList: instructions
            ));

        }

    }

}