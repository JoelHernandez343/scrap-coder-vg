// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {
    public class OperationDropMenuIconChanger : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeSprite operationIcon;

        // Lazy variables
        DropMenuController _dropMenu;
        DropMenuController dropMenu => _dropMenu ??= (GetComponent<DropMenuController>() as DropMenuController);

        // Methods
        void Start() {
            dropMenu.AddListener(() => ChangeIcon(operation: dropMenu.Value));
        }

        public void ChangeIcon(string operation) {
            if (operation == "a") {
                operationIcon.SetState("sum");
            } else if (operation == "s") {
                operationIcon.SetState("substraction");
            } else if (operation == "m") {
                operationIcon.SetState("multiplication");
            } else if (operation == "d") {
                operationIcon.SetState("division");
            }
        }

    }
}