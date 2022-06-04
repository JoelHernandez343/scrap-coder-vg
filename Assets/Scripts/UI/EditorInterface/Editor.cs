using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.GameInput;

namespace ScrapCoder.UI {
    public class Editor : MonoBehaviour {

        // Lazy variables
        public bool isVisible => gameObject.activeSelf;

        // State variables
        public bool isEditorOpenRemotely {
            private set;
            get;
        }

        public void SetVisible(bool visible, bool isEditorOpenRemotely = false) {

            if (visible) {
                InputController.instance.SetFocusOnContainer(gameObject);
            } else {
                InputController.instance.ClearFocusOfContainer();
            }

            gameObject.SetActive(visible);

            this.isEditorOpenRemotely = isEditorOpenRemotely;
        }

    }
}