using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ScrapCoder.UI {
    public class InputText : MonoBehaviour, InputManagment.IInputHandler {

        // Editor variables
        [SerializeField] TextMeshPro textMeshPro;

        [SerializeField] VisualNodes.NodeTransform cursorSprite;
        [SerializeField] Animator cursorAnimator;

        [SerializeField] VisualNodes.NodeTransform background;

        // State variables
        int cursor = 0;

        // Lazy state variables
        string text {
            get => textMeshPro.text;
            set => textMeshPro.text = value;
        }

        int? _previousTextWidth;
        int previousTextWidth {
            get => _previousTextWidth ??= currentTextWidth;
            set => _previousTextWidth = value;
        }

        // Lazy and other variables
        int currentTextWidth {
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

        const int lettersOffset = 6;
        const int originX = 2;

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

            Expand();
            MoveCursor(-1);
        }

        void AddCharacter(string character) {
            if (cursor == text.Length) {
                text += character;
            } else {
                text = text.Insert(cursor, character);
            }

            Expand();
            MoveCursor(1);
        }

        void Expand() {
            // Store old width
            previousTextWidth = currentTextWidth;

            // Refresh text dimensions
            textMeshPro.ForceMeshUpdate();

            var textDelta = ExpandTextBox();
            var delta = ExpandBackground(textDelta);

            // Update previous width
            previousTextWidth = currentTextWidth;

            // Update parents with delta
        }

        int ExpandBackground(int textDelta) {
            var delta = 0;
            var initWidth = background.initWidth - lettersOffset;

            var previous = previousTextWidth;
            var current = currentTextWidth;

            if (previous < initWidth && current <= initWidth) {
                delta = 0;
            } else if (previous <= initWidth && current > initWidth) {
                delta = current - initWidth;
            } else if (previous >= initWidth && current > initWidth) {
                delta = textDelta;
            } else if (previous >= initWidth && current <= initWidth) {
                delta = initWidth - previous;
            }

            background.Expand(dx: delta, smooth: true);

            return delta;
        }

        int ExpandTextBox() {
            var textDelta = currentTextWidth - previousTextWidth;
            textTransform.Expand(dx: textDelta);

            return textDelta;
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
                cursorSprite.SetPosition(
                    x: originX,
                    y: cursorSprite.y,
                    smooth: true,
                    endingCallback: () => cursorAnimator.SetBool("isMoving", false)
                );
            } else {
                var x = (int)System.Math.Round(textMeshPro.textInfo.characterInfo[cursor - 1].topRight.x);
                cursorSprite.SetPosition(
                    x: x - 2 + originX,
                    y: cursorSprite.y,
                    smooth: true,
                    endingCallback: () => cursorAnimator.SetBool("isMoving", false)
                );
            }

            // Change state AFTER possible previous callback is called
            cursorAnimator.SetBool("isMoving", true);
        }
    }
}
