// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class NegationBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer conditionContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new NegationInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                conditionContainer: conditionContainer
            );
        }

    }

    class NegationInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingCondition, EvaluatingCondition }

        // State variables
        Steps currentStep = Steps.PushingCondition;

        List<InterpreterElement> conditionList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => true;

        InterpreterElement condition => conditionList[0];

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingCondition) {
                PushingCondition();
            } else if (currentStep == Steps.EvaluatingCondition) {
                EvaluationCondition(conditionValue: argument);
            }

        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingCondition;
        }

        public override InterpreterElement NextStatement() => null;

        void PushingCondition() {
            // Debug.Log("Pushing condition");

            Executer.instance.PushNext(condition);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.EvaluatingCondition;
        }

        void EvaluationCondition(string conditionValue) {
            // Debug.Log($"Evaluating condition result: {value}");

            Executer.instance.ExecuteInmediately(argument: conditionValue == "true" ? "false" : "true");

            isFinished = true;
        }

        public NegationInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer conditionContainer
        ) : base(parentList, controllerReference) {

            conditionList.AddRange(InterpreterElementsFromContainer(
                container: conditionContainer,
                parentList: conditionList
            ));

        }

    }

}