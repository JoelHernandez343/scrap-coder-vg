// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {

    public class ScanInterpreter : InterpreterElement {

        // Internal types
        enum Steps { PushingInstruction, SettingVariable }

        // Editor variable
        [SerializeField] NodeContainer variableContainer;

        // State variables
        Steps currentStep;

        /// Lazy variables
        public override bool IsExpression => false;

        NodeController variable => variableContainer.First;
        string symbolName => variable.symbolName;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingInstruction) {
                PushinIntruction();
            } else {
                SetVariable(newValue: argument);
            }
        }

        void PushinIntruction() {
            SendInstruction.sendInstruction((int)Actions.Scan);
            currentStep = Steps.SettingVariable;
        }

        void SetVariable(string newValue) {
            SymbolTable.instance[symbolName].SetValue(newValue: newValue);
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingInstruction;
        }

    }
}