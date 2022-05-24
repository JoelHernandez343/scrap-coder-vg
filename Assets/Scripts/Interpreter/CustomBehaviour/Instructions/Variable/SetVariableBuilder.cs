// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class SetVariableBuilder : InterpreterElementBuilder {

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer valueContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new SetVariableInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                valueContainer: valueContainer,
                variableContainer: variableContainer
            );
        }

    }

    class SetVariableInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingValue, SettingVariable }

        // State variables
        Steps currentStep;

        List<InterpreterElement> valueList = new List<InterpreterElement>();
        List<InterpreterElement> variableList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement variable => variableList[0];
        InterpreterElement value => valueList[0];

        string variableSymbolName => variable.symbolName;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.SettingVariable) {
                SettingVariable(value: argument);
            }

        }

        void PushingValue() {
            Executer.instance.PushNext(value);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.SettingVariable;
        }

        void SettingVariable(string value) {
            SymbolTable.instance[variableSymbolName].SetValue(newValue: value);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingValue;
        }

        public SetVariableInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer valueContainer,
            NodeContainer variableContainer
        ) : base(parentList, controllerReference) {

            valueList.AddRange(InterpreterElementsFromContainer(
                container: valueContainer,
                parentList: valueList
            ));

            variableList.AddRange(InterpreterElementsFromContainer(
                container: variableContainer,
                parentList: variableList
            ));

        }

    }

}