// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScrapCoder.UI {
    public class DropMenuSelectButton : MonoBehaviour {

        // Editor variables
        [SerializeField] DropMenuList list;
        [SerializeField] ButtonController menuButtonPrefab;
        [SerializeField] DropMenuController dropMenuController;

        // Lazy variables
        ButtonController _button;
        ButtonController button => _button ??= GetComponent<ButtonController>();

        bool menuState = true;
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
                menuState = false;
            }));

            dropMenuController.PositionList();
        }

        void ToggleList() {
            menuState = !menuState;
            list.SetVisible(menuState);
        }

    }
}