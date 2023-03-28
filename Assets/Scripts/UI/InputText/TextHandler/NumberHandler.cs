using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ScrapCoder.UI {
    public class NumberHandler : MonoBehaviour, ITextHandler {

        string[] numbers = { "-", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        Regex form = new Regex("^-?\\d?\\d?$");

        public void HandleCharacterInput(InputText inputText) {
            var character = GetPressedCharacter();

            var newText = inputText.Value.Insert(inputText.Cursor, character);

            if (character == "" || !form.IsMatch(newText)) return;

            inputText.AddCharacter(character);

        }

        public void HandleDeleteCharacter(InputText inputText) { }

        string GetPressedCharacter() {
            var keyPressed = "";

            int character;
            for (character = 0; character < numbers.Length; ++character) {
                if (Input.GetKeyDown(numbers[character])) {
                    keyPressed = numbers[character];
                    break;
                }
            }

            return keyPressed;
        }

    }
}