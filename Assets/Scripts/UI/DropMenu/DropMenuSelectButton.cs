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

        List<string> options = new List<string> {
            "Texto",
            "Opcion2"
        };

        // Methods
        void InitializeList() {
            list.buttons = options.ConvertAll(option => ButtonController.Create(menuButtonPrefab, list.transform, true, option));
            list.SetButtons();

            var listWidth = list.ownTransform.width;
            var unionOffset = 12;
            var menuRightOffset = 8;
            var menuWidth = dropMenuController.ownTransform.width;

            list.ownTransform.SetPosition(
                x: (menuWidth - menuRightOffset) - (listWidth - unionOffset)
            );

            ToggleList();
        }

        void Start() {
            button.AddListener(ToggleList);
            InitializeList();
        }

        void ToggleList() {
            menuState = !menuState;
            list.SetVisible(menuState);
        }

    }
}