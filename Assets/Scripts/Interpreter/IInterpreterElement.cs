// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {

    public interface IInterpreterElement {
        bool IsFinished { get; }
        bool IsExpression { get; }

        NodeController Controller { get; }

        void Reset();
        void Execute(string answer);
        IInterpreterElement GetNextStatement();
    }

}
