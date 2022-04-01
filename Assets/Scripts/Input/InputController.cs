// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.InputManagment {

    public interface IInputHandler {
        void HandleInput();
        void GetRemoverOwnership(GameObject remover);
        void LoseFocus();
        void GetFocus();
        bool HasFocus();
    }

    public class InputController : MonoBehaviour {

        [SerializeField] Canvas canvas;
        [SerializeField] GameObject remover;

        // Lazy and other variables
        public static InputController instance {
            private set;
            get;
        }

        RectTransform _removerRectTransform;
        RectTransform removerRectTransform => _removerRectTransform ??= remover.GetComponent<RectTransform>();

        public IInputHandler handlerWithFocus;

        public void SetFocusOn(IInputHandler inputHandler) {
            handlerWithFocus = inputHandler;

            handlerWithFocus.GetFocus();
            handlerWithFocus.GetRemoverOwnership(remover);
        }

        public void ClearFocus() {
            handlerWithFocus = null;

            GetRemoverOwnership();
        }

        public void GetRemoverOwnership() {
            remover.transform.SetParent(canvas.transform);
            remover.SetActive(false);

            var localPosition = removerRectTransform.localPosition;
            localPosition.z = 0;

            removerRectTransform.localPosition = localPosition;
        }

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