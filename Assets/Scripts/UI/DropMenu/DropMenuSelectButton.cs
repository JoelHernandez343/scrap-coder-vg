// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class DropMenuSelectButton : MonoBehaviour {

        // Editor variables
        [SerializeField] DropMenuList list;
        [SerializeField] ButtonController menuButtonPrefab;
        [SerializeField] DropMenuController dropMenuController;

        // Lazy variables
        ButtonController _button;
        ButtonController button => _button ??= GetComponent<ButtonController>();

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        bool visibleMenuState = true;
        bool initializeList = true;

        List<string> options => dropMenuController.GetOptions();

        // Methods
        void Start() {
            button.AddListener(() => {
                ToggleList();
                if (initializeList) {
                    InitializeList();
                    initializeList = false;
                }

                // Set controller focus
                if (visibleMenuState) {
                    controller?.GetFocus();
                } else {
                    controller?.LoseFocus();
                }
            });

            ToggleList();
        }

        void InitializeList() {
            list.ClearButtons();
            list.buttons = options.ConvertAll(option => ButtonController.Create(menuButtonPrefab, list.transform, true, option));
            list.SetButtons();
            list.buttons.ForEach(button => button.AddListener(() => {
                // Refresh own text
                dropMenuController.ChangeOption(button.text, smooth: true);

                // Set state of button in normal
                button.SetState("normal");

                // Hide list
                list.SetVisible(false);
                visibleMenuState = false;

                // Lose focus
                controller?.LoseFocus();
            }));

            dropMenuController.PositionList();
        }

        void ToggleList() {
            visibleMenuState = !visibleMenuState;
            list.SetVisible(visibleMenuState);
        }

    }
}