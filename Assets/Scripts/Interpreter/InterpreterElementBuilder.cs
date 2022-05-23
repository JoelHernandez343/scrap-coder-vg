// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public abstract class InterpreterElementBuilder : MonoBehaviour {

        // Internal types
        enum VisualState {
            Pressed, NotPressed, Finished, Paused
        }

        // State variables
        public virtual bool IsFinished { get; protected set; }
        public abstract bool IsExpression { get; }

        public bool HasFocus { get; private set; }

        // Lazy and other variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        public NodeController Controller => ownTransform.controller;

        float timer = 0f;
        float waitTime = 0.25f;

        VisualState visualState;

        // Methods
        abstract public void Execute(string argument);

        void Update() {
            if (!HasFocus) return;

            timer += Time.deltaTime;

            if (timer >= waitTime) {
                timer -= waitTime;
                Flickering();
            }

            SetVisualState();
        }

        void SetVisualState() {
            if (visualState == VisualState.Pressed) {
                Controller.SetState("pressed");
            } else if (
                visualState == VisualState.NotPressed ||
                visualState == VisualState.Finished
            ) {
                Controller.SetState("normal");
            }
        }

        void Flickering() {
            if (visualState == VisualState.Pressed) {
                visualState = VisualState.NotPressed;
            } else if (visualState == VisualState.NotPressed) {
                visualState = VisualState.Pressed;
            }
        }

        public void PauseFlickering() {
            visualState = VisualState.Paused;
        }

        public void ReleaseVisualState() {
            visualState = HasFocus ? VisualState.Pressed : VisualState.Finished;
        }

        public void Reset() {
            timer = 0f;
            HasFocus = false;
            visualState = VisualState.Pressed;

            IsFinished = false;

            CustomReset();
        }

        protected virtual void CustomReset() { }

        public void GetFocus() {
            timer = 0f;
            HasFocus = true;
            visualState = VisualState.Pressed;
        }

        public void LoseFocus() {
            HasFocus = false;
            Controller.SetState("normal");
            visualState = VisualState.Finished;
        }

        public virtual InterpreterElementBuilder GetNextStatement() {
            try {
                return Controller.parentArray.Next(Controller)?.interpreterElement;
            } catch (System.Exception e) {
                Debug.LogError(e, this);
                Debug.LogError($"[{name}] This is fucked up");
                return null;
            }
        }

    }
}