// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class QwertyNumbersTextWithSearchHandler : MonoBehaviour, ITextHandler {

        // Editor variables
        [SerializeField] SelectGameMenuController selectGameMenuController; 

        // Methods
        public void HandleCharacterInput(InputText inputText) {
            var character = QwertyNumbersTextHandler.GetPressedCharacter();

            if (string.IsNullOrEmpty(character)) return;

            var newFilter = inputText.Value.Insert(inputText.Cursor, character);
            selectGameMenuController.FilterUsersBy(newFilter);

            inputText.AddCharacter(character);
        }

        public void HandleDeleteCharacter(InputText inputText){
            var newFilter = inputText.Value;
            selectGameMenuController.FilterUsersBy(newFilter);
        }
    }
}