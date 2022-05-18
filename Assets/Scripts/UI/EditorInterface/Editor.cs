using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.GameInput;

namespace ScrapCoder.UI {
    public class Editor : MonoBehaviour {

        public bool isVisible => gameObject.activeSelf;

        public void SetVisible(bool visible) {

            if (visible) {
                InputController.instance.SetFocusOnContainer(gameObject);
            } else {
                InputController.instance.ClearFocusOfContainer();
            }

            gameObject.SetActive(visible);

        }

    }
}