// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ConstantBooleanInterpreter : MonoBehaviour, IInterpreterElement {

        // Internal types
        enum TypeOfBoolean { True, False }

        // Editor variables
        [SerializeField] TypeOfBoolean typeOfBoolean;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool _isFinished = false;
        public bool IsFinished {
            get => _isFinished;
            set => _isFinished = value;
        }

        public bool IsExpression => true;
        public NodeController Controller => ownTransform.controller;

        // Methods
        public void Execute(string answer) {
            // Debug.Log("Returnig false");

            Executer.instance.ExecuteInmediately(
                answer: typeOfBoolean == TypeOfBoolean.True ? "true" : "false"
            );

            IsFinished = true;
        }

        public void Reset() {
            IsFinished = false;
        }

        public IInterpreterElement GetNextStatement() => null;
    }
}
