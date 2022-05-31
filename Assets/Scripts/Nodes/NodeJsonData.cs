// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.Game {

    public class NodeJsonData {
        // Internal type
        public class SerializablePosition {
            public int x;
            public int y;
        }

        public class NodePositionTuple {
            public NodeControllerTemplate nodeTemplate;
            public SerializablePosition position;
        }

        // Fields
        public SymbolTableTemplate symbolTableTemplate;
        public List<NodePositionTuple> nodesTuples;
    }
}