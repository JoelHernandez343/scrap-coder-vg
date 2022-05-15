// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.GameInput {

    public class InputController : MonoBehaviour {

        [SerializeField] GameObject remover;

        // Lazy and other variables
        public static InputController instance {
            private set;
            get;
        }

        RectTransform _removerRectTransform;
        RectTransform removerRectTransform => _removerRectTransform ??= remover.GetComponent<RectTransform>();

        Canvas canvas => InterfaceCanvas.instance.canvas;

        // State variables
        public IFocusable handlerWithFocus;

        int previousDepth;
        int previousSortingOrder;
        Transform previousParent;

        public GameObject containerWithFocus;

        public void SetFocusParentOnFocusable() {
            if (handlerWithFocus == null) return;

            previousDepth = handlerWithFocus.ownTransform.depth;
            previousParent = handlerWithFocus.ownTransform.transform.parent;
            previousSortingOrder = handlerWithFocus.ownTransform.sorter.sortingOrder;

            handlerWithFocus.ownTransform.transform.SetParent(InterfaceCanvas.instance.focusParent.transform);
            handlerWithFocus.ownTransform.depth = 0;
            handlerWithFocus.ownTransform.sorter.sortingOrder = 0;
        }

        public void RemoveFromFocusParent(Transform newParent = null, int? previousDepth = null, int? previousSortingOrder = null) {
            if (handlerWithFocus == null) return;

            handlerWithFocus.ownTransform.transform.SetParent(newParent ?? previousParent);
            handlerWithFocus.ownTransform.depth = previousDepth ?? this.previousDepth;
            handlerWithFocus.ownTransform.sorter.sortingOrder = previousSortingOrder ?? this.previousSortingOrder;
        }

        public void SetFocusOn(IFocusable focusable) {
            ClearFocus();

            handlerWithFocus = focusable;

            handlerWithFocus.GetRemoverOwnership(remover);
            handlerWithFocus.GetFocus();
        }

        public void ClearFocus() {
            if (handlerWithFocus == null) return;

            handlerWithFocus.LoseFocus();
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

        public void SetFocusOnContainer(GameObject container) {
            ClearFocusOfContainer();

            containerWithFocus = container;
        }

        public void ClearFocusOfContainer() {
            containerWithFocus = null;
        }

        public bool IsInputAvailable(bool ignoreContainer = false) {
            if (ignoreContainer) return handlerWithFocus == null;

            return handlerWithFocus == null && containerWithFocus == null;
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

        // Mask methods
        public float GetAxisRaw(string axisName, bool ignoreContainer = false) {
            return IsInputAvailable(ignoreContainer) ? Input.GetAxisRaw(axisName: axisName) : 0;
        }

        public bool GetButtonDown(string buttonName, bool ignoreContainer = false) {
            return IsInputAvailable(ignoreContainer) ? Input.GetButtonDown(buttonName: buttonName) : false;
        }

        public bool GetKey(string keyName, bool ignoreContainer = false) {
            return IsInputAvailable(ignoreContainer) ? Input.GetKey(name: keyName) : false;
        }

        public bool GetKey(KeyCode key, bool ignoreContainer = false) {
            return IsInputAvailable(ignoreContainer) ? Input.GetKey(key: key) : false;
        }

        public bool GetKeyDown(string keyName, bool ignoreContainer = false) {
            return IsInputAvailable(ignoreContainer) ? Input.GetKeyDown(name: keyName) : false;
        }

        public bool GetKeyDown(KeyCode key, bool ignoreContainer = false) {
            return IsInputAvailable(ignoreContainer) ? Input.GetKeyDown(key: key) : false;
        }

    }

}