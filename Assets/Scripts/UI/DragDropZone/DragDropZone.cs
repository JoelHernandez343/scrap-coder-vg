// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class DragDropZone : MonoBehaviour {

        // Internal types
        public enum Category { Neutral, Working, Erasing }

        // Editor variables
        [SerializeField] public Category category;
        [SerializeField] NodeShapeContainer shapeState;

        // State variables
        string currentState = "";

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        void Start() {
            shapeState?.SetState("normal");
        }

        public void SetState(string state) {

            if (currentState == state) return;

            currentState = state;
            shapeState?.SetState(state);
        }
    }

}