// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

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

        [SerializeField] GameObject removerParent;

        [SerializeField] List<VisualNodes.NodeTransform> itemsToExpand;

        [SerializeField] VisualNodes.NodeShape backgroundShape;

        [SerializeField] int initWidth;

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

        const int lettersOffset = 8;
        const int cursorLeftOffset = 2;
        const int textLeftOffset = 4;

        // Methods
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
            var delta = CalculateDelta(textDelta);

            itemsToExpand.ForEach(item => item?.Expand(dx: delta, smooth: true));

            // Update previous width
            previousTextWidth = currentTextWidth;

            // Update parents with delta
        }

        int CalculateDelta(int textDelta) {
            var delta = 0;
            var initWidth = this.initWidth - lettersOffset;

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

            return delta;
        }

        int ExpandTextBox() {
            var textDelta = currentTextWidth - previousTextWidth;
            textTransform.Expand(dx: textDelta);

            return textDelta;
        }

        void MoveCursorTo(int position) {
            var delta = position - cursor;
            MoveCursor(delta);
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
            Vector2 delta;

            if (cursor == 0) {
                delta = cursorSprite.SetPosition(
                    x: cursorLeftOffset,
                    y: cursorSprite.y,
                    smooth: true,
                    endingCallback: () => cursorAnimator.SetBool("isMoving", false)
                );
            } else {
                var x = (int)System.Math.Round(textMeshPro.textInfo.characterInfo[cursor - 1].topRight.x);
                delta = cursorSprite.SetPosition(
                    x: x + cursorLeftOffset,
                    y: cursorSprite.y,
                    smooth: true,
                    endingCallback: () => cursorAnimator.SetBool("isMoving", false)
                );
            }

            // Change state AFTER possible previous callback is called if were transition
            if (delta != Vector2.zero) {
                cursorAnimator.SetBool("isMoving", true);
            }
        }

        public void Click(float x) {
            if (text == "") return;

            if (x <= cursorLeftOffset) {
                MoveCursorTo(0);
            }

            for (var i = 0; i < textMeshPro.textInfo.characterCount; ++i) {
                var info = textMeshPro.textInfo.characterInfo[i];
                var left = info.topLeft.x + textLeftOffset;
                var right = info.topRight.x + textLeftOffset;

                var middle = (right - left) / 2 + left;

                if (x < middle) {
                    MoveCursorTo(i);
                    return;
                }
            }

            MoveCursorTo(text.Length);
        }

        void InputManagment.IInputHandler.GetRemoverOwnership(GameObject remover) {
            remover.transform.SetParent(removerParent.transform);
            remover.SetActive(true);

            var removerRectTransfrom = remover.GetComponent<RectTransform>();
            var localPosition = removerRectTransfrom.localPosition;
            localPosition.z = 0;

            removerRectTransfrom.localPosition = localPosition;
        }

        void InputManagment.IInputHandler.LoseFocus() {
            cursorAnimator.SetBool("isActive", false);

            backgroundShape.SetVisible(false);
        }

        void InputManagment.IInputHandler.GetFocus() {
            cursorAnimator.SetBool("isActive", true);

            backgroundShape.SetVisible(true);
        }

        bool InputManagment.IInputHandler.HasFocus() {
            return InputManagment.InputController.instance.handlerWithFocus == (InputManagment.IInputHandler)this;
        }
    }
}
