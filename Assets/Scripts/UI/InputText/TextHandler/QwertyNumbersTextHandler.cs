// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.UI {
    public class QwertyNumbersTextHandler : MonoBehaviour, ITextHandler {

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

        string[] symbols = {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
        }; 

        public void HandleCharacterInput(InputText inputText) {
            var character = GetPressedCharacter();
            inputText.AddCharacter(character);
        }

        string GetPressedCharacter() {
            var pressedKey = LookForInQwerty();

            if (pressedKey == "") {
                pressedKey = LookForInNumbers();
            }

            return pressedKey;
        }

        string LookForInQwerty() {
            var pressedKey = "";

            int letter;
            for (letter = 0; letter < qwerty.Length; ++letter) {
                if (!Input.GetKeyDown(qwerty[letter])) continue;

                pressedKey = qwerty[letter];

                if (pressedKey == ";") {
                    pressedKey = "ñ";
                }

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    pressedKey = qwertyUpperCase[letter];
                }

                break;
            }

            return pressedKey;
        }

        string LookForInNumbers() {
            var pressedKey = "";

            int letter;
            for (letter = 0; letter < symbols.Length; ++letter) {
                if (!Input.GetKeyDown(symbols[letter])) continue;

                pressedKey = symbols[letter];

                break;
            }

            return pressedKey;
        }
    }
}