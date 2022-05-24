// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public abstract class InterpreterElementBuilder : MonoBehaviour {

        // Lazy and other variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        public NodeController Controller => ownTransform.controller;

        // Methods
        public abstract InterpreterElement GetInterpreterElement(
            List<InterpreterElement> parentList
        );

    }
}