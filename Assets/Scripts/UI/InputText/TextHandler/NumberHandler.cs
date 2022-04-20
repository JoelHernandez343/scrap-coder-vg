using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ScrapCoder.UI {
    public class NumberHandler : MonoBehaviour, ITextHandler {

        string[] numbers = { "-", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        Regex minusAndNumber = new Regex("^-\\d$");
        Regex minusAndTwoNumbers = new Regex("^-\\d\\d$");

        Regex number = new Regex("^\\d$");
        Regex twoNumbers = new Regex("^\\d\\d$");

        public void HandleCharacterInput(InputText inputText) {
            var character = GetPressedCharacter();

            if (
                character == "" ||
                (inputText.Value == "" && character == "0") ||
                (inputText.Value == "-" && (character == "0" || character == "-")) ||
                (minusAndNumber.IsMatch(inputText.Value) && character == "-") ||
                minusAndTwoNumbers.IsMatch(inputText.Value) ||
                (number.IsMatch(inputText.Value) && character == "-") ||
                twoNumbers.IsMatch(inputText.Value)
            ) {
                return;
            }

            inputText.AddCharacter(character);

        }

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