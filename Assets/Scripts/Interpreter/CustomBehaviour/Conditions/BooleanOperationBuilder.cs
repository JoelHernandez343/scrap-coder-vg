// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class BooleanOperationBuilder : InterpreterElementBuilder {

        // Internal types
        public enum Operation { Or, And }

        // Editor variables
        [SerializeField] NodeContainer leftContainer;
        [SerializeField] NodeContainer rightContainer;

        [SerializeField] Operation operation;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new BooleanOperationInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                leftConditionContainer: leftContainer,
                rightConditionContainer: rightContainer,
                operation: operation
            );
        }

    }

    class BooleanOperationInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingLeftCondition, EvaluatingLeftCondition, PushingRightCondition, EvaluatingRightCondition }

        // State variables
        Steps currentStep = Steps.PushingLeftCondition;

        List<InterpreterElement> leftList = new List<InterpreterElement>();
        List<InterpreterElement> rightList = new List<InterpreterElement>();

        BooleanOperationBuilder.Operation operation;

        // Lazy variables
        public override bool isExpression => true;

        string leftValue;
        string rightValue;

        InterpreterElement leftCondition => leftList[0];
        InterpreterElement rightCondition => rightList[0];

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingLeftCondition) {
                PushingCondition("left");
            } else if (currentStep == Steps.EvaluatingLeftCondition) {
                StoreValue(condition: "left", conditionValue: argument);
                EvaluationCondition(condition: "left");
            } else if (currentStep == Steps.PushingRightCondition) {
                PushingCondition("right");
            } else if (currentStep == Steps.EvaluatingRightCondition) {
                StoreValue(condition: "right", conditionValue: argument);
                EvaluationCondition(condition: "right");
            }

        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingLeftCondition;
        }

        public override InterpreterElement NextStatement() => null;

        void PushingCondition(string condition) {
            var conditionToPush = condition == "left"
                ? leftCondition
                : rightCondition;

            Executer.instance.PushNext(conditionToPush);
            Executer.instance.ExecuteInmediately();

            currentStep = condition == "left"
                ? Steps.EvaluatingLeftCondition
                : Steps.EvaluatingRightCondition;
        }

        void EvaluationCondition(string condition) {

            var conditionValue = condition == "left" ? leftValue : rightValue;

            if (
                operation == BooleanOperationBuilder.Operation.Or ||
                operation == BooleanOperationBuilder.Operation.And
            ) {
                var finisher = operation == BooleanOperationBuilder.Operation.Or
                    ? "true" : "false";
                var opposite = finisher == "true"
                    ? "false" : "true";

                if (conditionValue == finisher) {
                    isFinished = true;
                    Executer.instance.ExecuteInmediately(argument: finisher);
                } else {
                    currentStep = Steps.PushingRightCondition;

                    if (condition == "left") {
                        Executer.instance.ExecuteInmediately();
                    } else {
                        isFinished = true;
                        Executer.instance.ExecuteInmediately(argument: opposite);
                    }
                }
            }

        }

        void StoreValue(string condition, string conditionValue) {
            if (condition == "left") {
                leftValue = conditionValue;
            } else {
                rightValue = conditionValue;
            }
        }

        public BooleanOperationInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer leftConditionContainer,
            NodeContainer rightConditionContainer,
            BooleanOperationBuilder.Operation operation
        ) : base(parentList, controllerReference) {

            leftList.AddRange(InterpreterElementsFromContainer(
                container: leftConditionContainer,
                parentList: leftList
            ));

            rightList.AddRange(InterpreterElementsFromContainer(
                container: rightConditionContainer,
                parentList: rightList
            ));

            this.operation = operation;

        }

    }

}
