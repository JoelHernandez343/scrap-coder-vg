// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class GetLengthOfArrayInterpreter : InterpreterElement {

        // Editor variables
        [SerializeField] NodeContainer arrayContainer;

        // Lazy variables
        public override bool IsExpression => true;

        NodeController array => arrayContainer.First;

        string symbolName => array.symbolName;

        // Methods
        public override void Execute(string argument) {
            var arrayLength = SymbolTable.instance[symbolName].ArrayLength;

            Executer.instance.ExecuteInmediately(argument: $"{arrayLength}");

            IsFinished = true;
        }

    }
}