// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeDropHandler {
        // Properties
        bool IsActive { get; }
        Transform Transform { get; }
        NodeTransform OwnTransform { get; }

        // Methods
        bool OnDrop(NodeZone incomingZone);
        void OnTriggerEnter2D(Collider2D collider);
        void OnTriggerExit2D(Collider2D collider);
    }

}