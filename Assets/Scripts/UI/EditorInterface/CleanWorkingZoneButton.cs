// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class CleanWorkingZoneButton : MonoBehaviour {

        ButtonController _button;
        ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        void Start() {

            button.AddListener(() => {
                if (Executer.instance.isRunning) {
                    MessagesController.instance.AddMessage(
                        message: "No puedes limpiar los nodos mientras el ejecutor trabaja.",
                        type: MessageType.Warning
                    );
                    return;
                }

                SymbolTable.instance.CleanReferencesWihoutParent();
            });

        }
    }
}