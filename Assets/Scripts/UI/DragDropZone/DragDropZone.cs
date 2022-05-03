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

        // State variables
        string previousState = "";
        string currentState = "";

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        public void SetState(string state) {
            currentState = state;

            if (currentState == "over" && previousState != currentState) {
                // Here switch to over state
                // Debug.Log("Over");
            } else if (currentState == "normal" && previousState != currentState) {
                // Here switch to normal state
                // Debug.Log("Normal");
            }

            previousState = currentState;
        }
    }

}