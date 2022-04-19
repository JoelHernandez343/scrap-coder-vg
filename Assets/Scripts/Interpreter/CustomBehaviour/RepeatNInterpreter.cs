// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class RepeatNInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { ComparingWithVariable, ExecutingInstructions }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer instructionsContainer;

        // State variables
        Steps currentStep = Steps.ComparingWithVariable;
        int repetition = 0;

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

        NodeController variable => variableContainer.array.First;
        NodeController firstInstruction => instructionsContainer.array.First;

        // Methods
        public void Execute(string answer) {

            if (currentStep == Steps.ComparingWithVariable) {
                ComparingWithVariable();
            } else if (currentStep == Steps.ExecutingInstructions) {
                ExecutingInstructions();
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.ComparingWithVariable;
            repetition = 0;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

        void ComparingWithVariable() {
            var value = System.Int32.Parse(SymbolTable.instance[variable.symbolName].value);

            if (repetition < value) {
                repetition += 1;
                currentStep = Steps.ExecutingInstructions;
            } else {
                IsFinished = true;
            }

            Executer.instance.ExecuteInNextFrame();
        }

        void ExecutingInstructions() {
            // Debug.Log("Executing instructions");

            Executer.instance.PushNext(firstInstruction.interpreterElement);
            Executer.instance.ExecuteInNextFrame();

            currentStep = Steps.ComparingWithVariable;
        }

    }

}