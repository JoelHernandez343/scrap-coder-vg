// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class SetVariableInterpreter : InterpreterElement {

        // Private types
        enum Steps { PushingValue, SettingVariable }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer valueContainer;

        // State variables
        Steps currentStep;

        // Lazy variables
        public override bool IsExpression => false;

        NodeController variable => variableContainer.First;
        NodeController value => valueContainer.First;

        string symbolName => variable.symbolName;

        // Methods
        public override void Execute(string argument) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.SettingVariable) {
                SettingVariable(value: argument);
            }

        }

        void PushingValue() {
            Executer.instance.PushNext(value.interpreterElement);
            Executer.instance.ExecuteInmediately();

            currentStep = Steps.SettingVariable;
        }

        void SettingVariable(string value) {
            SymbolTable.instance[symbolName].value = value;
            Executer.instance.ExecuteInmediately();

            IsFinished = true;
        }

        protected override void CustomReset() {
            currentStep = Steps.PushingValue;
        }

    }

}