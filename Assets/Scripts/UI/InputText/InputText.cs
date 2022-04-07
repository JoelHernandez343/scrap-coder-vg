// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class InputText : MonoBehaviour, InputManagment.IInputHandler, INodeExpandable {

        // Editor variables
        [SerializeField] NodeTransform cursorSprite;
        [SerializeField] Animator cursorAnimator;

        [SerializeField] GameObject removerParent;

        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] NodeShape backgroundShape;

        [SerializeField] int initWidth;

        [SerializeField] ExpandableText expandableText;

        [SerializeField] public NodeTransform pieceToExpand;

        // State variables
        int cursor = 0;

        // Lazy state variables
        string _text = null;
        string text {
            get => _text ??= expandableText.text;
            set => _text = value;
        }

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

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        NodeTransform INodeExpandable.PieceToExpand => pieceToExpand;
        bool INodeExpandable.ModifyHeightOfPiece => false;
        bool INodeExpandable.ModifyWidthOfPiece => true;

        // Constants
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

            ExpandByText();
            MoveCursor(-1);
        }

        void AddCharacter(string character) {
            if (cursor == text.Length) {
                text += character;
            } else {
                text = text.Insert(cursor, character);
            }

            ExpandByText();
            MoveCursor(1);
        }

        void ExpandByText() {
            // Change text and get new delta
            var dx = expandableText.ChangeText(
                newText: text,
                minWidth: initWidth,
                lettersOffset: lettersOffset
            );

            // Expand items
            itemsToExpand.ForEach(item => item?.Expand(dx: dx, smooth: true));

            // Update parents with delta
            controller?.AdjustParts(expandable: this, delta: (dx, 0), smooth: true);
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
                var x = (int)System.Math.Round(expandableText.characterInfo[cursor - 1].topRight.x);
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

            for (var i = 0; i < expandableText.characterCount; ++i) {
                var info = expandableText.characterInfo[i];
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
