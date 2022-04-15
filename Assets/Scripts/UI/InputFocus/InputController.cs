// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.InputManagment {

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

        public IFocusable handlerWithFocus;

        public void SetFocusOn(IFocusable focusable) {
            ClearFocus();

            handlerWithFocus = focusable;

            handlerWithFocus.GetRemoverOwnership(remover);
            handlerWithFocus.GetFocus();
        }

        public void ClearFocus() {
            handlerWithFocus?.LoseFocus();
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
            if (handlerWithFocus is IInputHandler inputHandler) {
                inputHandler.HandleInput();
            }
        }
    }

}