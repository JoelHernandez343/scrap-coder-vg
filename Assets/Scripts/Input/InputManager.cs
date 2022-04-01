using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.InputManagment {

    public interface IInputHandler {
        void HandleInput();
    }

    public class InputManager : MonoBehaviour {

        // Lazy and other variables
        public static InputManager instance {
            private set;
            get;
        }

        IInputHandler handlerWithFocus;

        public void SetFocusOn(IInputHandler inputHandler) => handlerWithFocus = inputHandler;

        public void ClearFocus() => handlerWithFocus = null;

        void Awake() {
            if (instance != null) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        void Update() {
            handlerWithFocus?.HandleInput();
        }
    }

}