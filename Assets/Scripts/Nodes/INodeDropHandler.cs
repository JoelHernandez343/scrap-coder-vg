// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeDropHandler {
        bool OnDrop(NodeZone incomingZone, NodeZone ownZone);
    }

}