using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ScrapCoder.UI {
    public class InputText : MonoBehaviour, InputManagment.IInputHandler {

        // Editor variables
        [SerializeField] TextMeshPro textMeshPro;
        [SerializeField] VisualNodes.NodeTransform cursorSprite;

        // State variables
        int cursor = 0;

        // Lazy state variables
        string text {
            get => textMeshPro.text;
            set => textMeshPro.text = value;
        }

        // Lazy and other variables
        int lastRight {
            get {
                if (textMeshPro.textInfo.characterCount == 0) return 0;

                return (int)System.Math.Round(
                    textMeshPro.textInfo.characterInfo[textMeshPro.textInfo.characterCount - 1].topRight.x
                );
            }
        }

        VisualNodes.NodeTransform _textTransform;
        VisualNodes.NodeTransform textTransform
            => _textTransform ??= textMeshPro.GetComponent<VisualNodes.NodeTransform>();

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

        const KeyCode delete = KeyCode.Backspace;
        const KeyCode right = KeyCode.RightArrow;
        const KeyCode left = KeyCode.LeftArrow;
        const KeyCode shiftLeft = KeyCode.LeftShift;
        const KeyCode shiftRight = KeyCode.RightShift;

        const int letterWidth = 8;

        // Methods
        void Start() {
            InputManagment.InputManager.instance.SetFocusOn(this);
        }

        void InputManagment.IInputHandler.HandleInput() {

            if (!Input.anyKeyDown) return;

            if (Input.GetKeyDown(delete)) {
                DeleteCharacterFromCursor();
            } else if (Input.GetKeyDown(right)) {
                MoveCursor(1);
            } else if (Input.GetKeyDown(left)) {
                MoveCursor(-1);
            } else if (GetPressedCharacter() is var character && character != "") {
                AddCharacter(character);
            }
        }

        string GetPressedCharacter() {
            var keyPressed = "";

            int letter;
            for (letter = 0; letter < qwerty.Length; ++letter) {
                if (Input.GetKeyDown(qwerty[letter])) {
                    keyPressed = qwerty[letter];
                    break;
                }
            }

            if (keyPressed == "") return keyPressed;

            if (keyPressed == ";") keyPressed = "ñ";

            if (Input.GetKey(shiftLeft) || Input.GetKey(shiftRight)) {
                keyPressed = qwertyUpperCase[letter];
            }

            return keyPressed;
        }

        void DeleteCharacterFromCursor() {
            if (cursor == 0) return;

            text = text.Remove(cursor - 1, 1);

            ExpandTextBox();
            MoveCursor(-1);
        }

        void AddCharacter(string character) {
            if (cursor == text.Length) {
                text += character;
            } else {
                text = text.Insert(cursor, character);
            }

            ExpandTextBox();
            MoveCursor(1);
        }

        void ExpandTextBox() {
            var previousRight = lastRight;
            textMeshPro.ForceMeshUpdate();
            var currentRight = lastRight;

            var dx = currentRight - previousRight;
            textTransform.Expand(dx: dx);
        }

        void MoveCursor(int delta) {
            if (cursor + delta < 0) {
                cursor = 0;
            } else if (cursor + delta > text.Length) {
                cursor = text.Length;
            } else {
                cursor += delta;
            }

            RenderCursor();
        }

        void RenderCursor() {
            if (cursor == 0) {
                cursorSprite.SetPosition(x: -2);
            } else {
                var x = (int)System.Math.Round(textMeshPro.textInfo.characterInfo[cursor - 1].topRight.x);
                cursorSprite.SetPosition(x: x - 2);
            }
        }
    }
}
