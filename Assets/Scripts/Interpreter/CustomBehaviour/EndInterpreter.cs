// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class EndInterpreter : MonoBehaviour, IInterpreterElement {

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public bool IsFinished => true;

        public bool IsExpression => false;
        public NodeController Controller => ownTransform.controller;

        // Methods
        public void Execute(string answer) { }
        public void Reset() { }

        public IInterpreterElement GetNextStatement() => null;
    }
}