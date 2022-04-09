// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.InputManagment;

namespace ScrapCoder.UI {
    public class DropMenuSelectButton : MonoBehaviour {

        // Editor variables
        [SerializeField] DropMenuList list;
        [SerializeField] DropMenuController dropMenuController;

        // Lazy variables
        ButtonController _button;
        ButtonController button => _button ??= GetComponent<ButtonController>();

        // Methods
        void Start() {
            button.AddListener(() => {
                // Set controller focus
                if (!(dropMenuController as IFocusable).HasFocus()) {
                    InputController.instance.SetFocusOn(dropMenuController);
                } else {
                    InputController.instance.ClearFocus();
                }
            });

            list.SetVisible(false);
        }
    }
}