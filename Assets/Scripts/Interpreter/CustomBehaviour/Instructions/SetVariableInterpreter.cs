// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class SetVariableInterpreter : MonoBehaviour, IInterpreterElement {

        // Private types
        enum Steps { PushingValue, SettingVariable }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer valueContainer;

        // State variables
        Steps currentStep;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool _isFinished = false;
        public bool IsFinished {
            get => _isFinished;
            set => _isFinished = value;
        }

        public bool IsExpression => false;
        public NodeController Controller => ownTransform.controller;

        NodeController variable => variableContainer.First;
        NodeController value => valueContainer.First;

        // Methods
        public void Execute(string answer) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.SettingVariable) {
                SettingVariable(answer);
            }

        }

        void PushingValue() {
            Executer.instance.PushNext(value.interpreterElement);
            Executer.instance.ExecuteInNextFrame();

            currentStep = Steps.SettingVariable;
        }

        void SettingVariable(string value) {
            SymbolTable.instance[variable.symbolName].value = value;
            Executer.instance.ExecuteInNextFrame();

            IsFinished = true;
        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingValue;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

    }

}