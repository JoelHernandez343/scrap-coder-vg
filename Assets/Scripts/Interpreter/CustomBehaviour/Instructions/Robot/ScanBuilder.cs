// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class ScanBuilder : InterpreterElementBuilder {

        // Editor variable
        [SerializeField] NodeContainer variableContainer;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ScanInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                variableContainer: variableContainer
            );
        }

    }

    class ScanInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingInstruction, SettingVariable }

        // State variables
        Steps currentStep;

        List<InterpreterElement> variableList = new List<InterpreterElement>();

        // Lazy variables
        public override bool isExpression => false;

        InterpreterElement variable => variableList[0];
        string variableSymbolName => variable.symbolName;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingInstruction) {
                PushingIntruction();
            } else {
                SetVariable(newValue: argument);
            }
        }

        void PushingIntruction() {
            SendInstruction.sendInstruction((int)Actions.Scan);
            currentStep = Steps.SettingVariable;
        }

        void SetVariable(string newValue) {
            SymbolTable.instance[variableSymbolName].SetValue(newValue: newValue);
            Executer.instance.ExecuteInmediately();

            isFinished = true;
        }

        protected override void CustomResetState() {
            currentStep = Steps.PushingInstruction;
        }

        public ScanInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            NodeContainer variableContainer
        ) : base(parentList, controllerReference) {

            variableList.AddRange(InterpreterElementsFromContainer(
                container: variableContainer,
                parentList: variableList
            ));

        }

    }
}