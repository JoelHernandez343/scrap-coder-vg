// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ScrapCoder.VisualNodes;
using ScrapCoder.InputManagment;

namespace ScrapCoder.UI {
    public class InputText : MonoBehaviour, IInputHandler, IFocusable, INodeExpanded, INodeExpander {

        // Editor variables
        [SerializeField] int characterLimit = -1;

        [SerializeField] NodeTransform cursorSprite;
        [SerializeField] Animator cursorAnimator;

        [SerializeField] GameObject removerParent;

        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] NodeShape backgroundShape;

        [SerializeField] int initWidth;

        [SerializeField] ExpandableText expandableText;

        [SerializeField] public NodeTransform pieceToExpand;

        [SerializeField] Component textHandler;

        // State variables
        int cursor = 0;

        List<System.Action> listeners = new List<System.Action>();

        // Lazy state variables
        string _text = null;
        string text {
            get => _text ??= expandableText.text;
            set => _text = value;
        }

        bool isFull => characterLimit > 0 && text.Length == characterLimit;

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
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        public NodeController controller => ownTransform?.controller;

        public string Value {
            get => text;
            set {
                text = value;
                // ExpandByText(smooth: true);
                // MoveCursorTo(0);
            }
        }

        public int Cursor {
            get => cursor;
            set => MoveCursorTo(value);
        }

        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;
        bool INodeExpanded.ModifyHeightOfPiece => false;
        bool INodeExpanded.ModifyWidthOfPiece => true;

        // Constants
        const int lettersOffset = 8;
        const int cursorLeftOffset = 2;
        const int textLeftOffset = 4;

        // Methods
        void InputManagment.IInputHandler.HandleInput() {

            if (!Input.anyKeyDown) return;

            if (Input.GetKeyDown(KeyCode.Backspace)) {
                DeleteCharacterFromCursor();
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                MoveCursor(1);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                MoveCursor(-1);
            } else if (Input.GetKeyDown(KeyCode.Return)) {
                Execute();
            } else if (textHandler is ITextHandler customTextHandler) {
                customTextHandler.HandleCharacterInput(this);
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

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                keyPressed = qwertyUpperCase[letter];
            }

            return keyPressed;
        }

        void DeleteCharacterFromCursor() {
            if (cursor == 0) return;

            text = text.Remove(cursor - 1, 1);

            ExpandByText(smooth: true);
            MoveCursor(-1);
        }

        public void AddCharacter(string character) {
            if (isFull) return;

            if (cursor == text.Length) {
                text += character;
            } else {
                text = text.Insert(cursor, character);
            }

            ExpandByText(smooth: true);
            MoveCursor(1);
        }

        void ExpandByText(bool smooth = false) {
            // Change text and get new delta
            var dx = expandableText.ChangeText(
                newText: text,
                minWidth: initWidth,
                lettersOffset: lettersOffset
            );

            // Expand items
            ownTransform.Expand(dx: dx, smooth: smooth);

            // Update parent with delta
            ownTransform.expandable?.Expand(dx: dx, smooth: smooth, expanded: this);
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

        void IFocusable.GetRemoverOwnership(GameObject remover) {
            remover.transform.SetParent(removerParent.transform);
            remover.SetActive(true);

            var removerRectTransfrom = remover.GetComponent<RectTransform>();
            var localPosition = removerRectTransfrom.localPosition;
            localPosition.z = 0;

            removerRectTransfrom.localPosition = localPosition;
        }

        void IFocusable.GetFocus() {
            cursorAnimator.SetBool("isActive", true);
            backgroundShape.SetVisible(true);

            if (controller == null) {
                ownTransform.Raise(depthLevels: 10);
            } else {
                var thisDepthLevels = ownTransform.depthLevelsToThisTransform();
                var globalDepthevels = ownTransform.controller?.lastController.ownTransform.depthLevels ?? 0;

                var raiseDepthLevels =
                    (globalDepthevels > thisDepthLevels
                        ? globalDepthevels - thisDepthLevels
                        : 0) + HierarchyController.instance.globalRaiseDiff + 10;

                ownTransform.Raise(depthLevels: raiseDepthLevels);

                controller?.GetFocus();
            }
        }

        void IFocusable.LoseFocus() {
            cursorAnimator.SetBool("isActive", false);

            backgroundShape.SetVisible(false);

            ownTransform.ResetRenderOrder(depthLevels: 1);
            controller?.LoseFocus();
        }

        bool IFocusable.HasFocus() {
            return InputManagment.InputController.instance.handlerWithFocus == (IFocusable)this;
        }

        public void AddListener(System.Action listener) => listeners.Add(listener);

        public bool RemoveListener(System.Action listener) => listeners.Remove(listener);

        public void Clear() {
            Value = "";
        }

        public void Execute() {
            listeners.ForEach(listener => listener());
        }

        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded _) {
            itemsToExpand.ForEach(item => item?.Expand(dx: dx, smooth: smooth));

            return (dx, dy);
        }

        void Start() {
            AddListener(() => InputController.instance.ClearFocus());
        }
    }
}
