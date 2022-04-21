// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public class RepeatNInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum Steps { PushingValue, EvaluatingValue, ExecutingInstructions }

        // Editor variables
        [SerializeField] NodeContainer variableContainer;
        [SerializeField] NodeContainer instructionsContainer;

        // State variables
        Steps currentStep = Steps.PushingValue;
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

        NodeController value => variableContainer.array.First;
        NodeController firstInstruction => instructionsContainer.array.First;

        // Methods
        public void Execute(string answer) {

            if (currentStep == Steps.PushingValue) {
                PushingValue();
            } else if (currentStep == Steps.EvaluatingValue) {
                EvaluatingValue(answer);
            } else if (currentStep == Steps.ExecutingInstructions) {
                ExecutingInstructions();
            }

        }

        public void Reset() {
            IsFinished = false;
            currentStep = Steps.PushingValue;
            repetition = 0;
        }

        public IInterpreterElement GetNextStatement() {
            return Controller.parentArray.Next(Controller)?.interpreterElement;
        }

        void PushingValue() {
            Executer.instance.PushNext(value.interpreterElement);
            Executer.instance.ExecuteInNextFrame();

            currentStep = Steps.EvaluatingValue;
        }

        void EvaluatingValue(string value) {
            var number = System.Int32.Parse(value);

            if (repetition < number) {
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

            currentStep = Steps.PushingValue;
        }

    }

}