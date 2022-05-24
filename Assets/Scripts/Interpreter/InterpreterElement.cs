// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public abstract class InterpreterElement {

        // Internal types
        enum VisualState { Pressed, NotPressed, Finished, Paused }

        // State variables
        public List<InterpreterElement> parentList { get; private set; }

        public string symbolName { get; private set; }
        public NodeType type { get; private set; }

        // Mechanism to ignore if _controllerRef becomes missing :(
        NodeController _controllerRef;
        public NodeController controllerReference {
            get {
                if (_controllerRef == null) {
                    Debug.LogError($"[{symbolName}] Reference become missing");
                    return null;
                }

                return _controllerRef;
            }
            private set => _controllerRef = value;
        }

        public virtual bool isFinished { get; protected set; }
        public abstract bool isExpression { get; }

        VisualState visualState;

        public List<List<InterpreterElement>> listOfChildren = new List<List<InterpreterElement>>();

        // Lazy variables
        int? _index;
        int index => _index ??= parentList?.IndexOf(this) ?? -1;

        // Methods
        public InterpreterElement(
            List<InterpreterElement> parentList,
            NodeController controllerReference
        ) {
            this.parentList = parentList;
            this.controllerReference = controllerReference;

            this.type = controllerReference.type;
            this.symbolName = controllerReference.symbolName;
        }

        abstract public void Execute(string argument);

        public void SetVisualState() {
            if (visualState == VisualState.Pressed) {
                controllerReference?.SetState("pressed");
            } else if (
                visualState == VisualState.NotPressed ||
                visualState == VisualState.Finished
            ) {
                controllerReference?.SetState("normal");
            }
        }

        public void ChangeFlickeringState() {
            if (visualState == VisualState.Pressed) {
                visualState = VisualState.NotPressed;
            } else if (visualState == VisualState.NotPressed) {
                visualState = VisualState.Pressed;
            }
        }

        public void ResetState() {
            visualState = VisualState.Pressed;

            isFinished = false;

            CustomResetState();
        }

        protected virtual void CustomResetState() { }

        public void GetFocus() {
            visualState = VisualState.Pressed;
        }

        public void LoseFocus() {
            visualState = VisualState.Finished;
            controllerReference?.SetState("normal");
        }

        public virtual InterpreterElement NextStatement() {
            return index < parentList.Count - 1 && index >= 0
                ? parentList[index + 1]
                : null;
        }

        protected List<InterpreterElement> InterpreterElementsFromContainer(
            NodeContainer container,
            List<InterpreterElement> parentList
        ) {
            listOfChildren.Add(parentList);

            return container.array.nodes.ConvertAll(
                n => n.GetInterpreterElement(parentList: parentList)
            );
        }

    }
}