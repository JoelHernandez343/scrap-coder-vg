using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ScrapCoder.UI {
    public class SymbolNameHandler : MonoBehaviour, ITextHandler {

        public static Regex validForm = new Regex("^[a-zA-ZáéíóúñÁÉÍÓÚÑü]+(\\d*[a-zA-ZáéíóúñÁÉÍÓÚÑü]*)*$");

        string[] numbers = { "-", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        string[] qwerty = {
            "a",  "b",  "c",  "d",  "e",  "f",
            "g",  "h",  "i",  "j",  "k",  "l",
            "m",  "n",  "o",  "p",  "q",  "r",
            "s",  "t",  "u",  "v",  "w",  "x",
            "y",  "z", ";",
        };

        string[] qwertyUpperCase = {
            "A",  "B",  "C",  "D",  "E",  "F",
            "G",  "H",  "I",  "J",  "K",  "L",
            "M",  "N",  "O",  "P",  "Q",  "R",
            "S",  "T",  "U",  "V",  "W",  "X",
            "Y",  "Z", "Ñ"
        };


        public void HandleCharacterInput(InputText inputText) {
            var character = GetPressedCharacter();

            var newText = inputText.Value.Insert(inputText.Cursor, character);

            if (character == "" || !validForm.IsMatch(newText)) return;

            inputText.AddCharacter(character);
        }

        public void HandleDeleteCharacter(InputText inputText) { }


        string GetPressedCharacter() {
            var (character, index) = GetCharacterFromCategory(qwerty);

            if (character != "") {
                if (character == ";") character = "ñ";

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    character = qwertyUpperCase[index];
                }

                return character;
            }

            (character, index) = GetCharacterFromCategory(numbers);

            return character;
        }

        (string character, int characterIndex) GetCharacterFromCategory(string[] category) {
            for (var i = 0; i < category.Length; ++i) {
                if (Input.GetKeyDown(category[i])) {
                    return (character: category[i], characterIndex: i);
                }
            }

            return ("", -1);
        }

    }
}