// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.InputManagment;

namespace ScrapCoder.UI {
    public class DropMenuController : MonoBehaviour, INodeExpander, IFocusable, INodeExpanded {

        // Editor variables
        [SerializeField] List<DropMenuOption> options;
        [SerializeField] ExpandableText text;

        [SerializeField] List<NodeTransform> itemsToExpand;
        [SerializeField] List<NodeTransform> itemsToRight;

        [SerializeField] public NodeTransform pieceToExpand;

        [SerializeField] DropMenuList list;

        [SerializeField] GameObject removerParent;

        [SerializeField] ButtonController menuButtonPrefab;

        [SerializeField] public bool controlledByExecuter = false;

        [SerializeField] bool showOptionValue = false;

        // State variables
        [SerializeField] string _value = "";
        public string Value {
            get => _value;
            private set => _value = value;
        }

        List<System.Action> listeners = new List<System.Action>();

        bool initializeList = true;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;
        bool INodeExpanded.ModifyHeightOfPiece => false;
        bool INodeExpanded.ModifyWidthOfPiece => true;

        // Methods
        void Start() {
            if (options.Count > 0 && Value == "") {
                ChangeOption(newOption: options[0], executeListeners: false);
            }
        }

        public void ChangeOption(DropMenuOption newOption, bool smooth = false, bool executeListeners = true) {
            Value = newOption.value;

            var dx = text.ChangeText(
                newText: showOptionValue ? newOption.value : newOption.text,
                minWidth: ownTransform.minWidth,
                lettersOffset: 20
            );

            ownTransform.Expand(dx: dx, smooth: smooth);

            ownTransform.expandable?.Expand(dx: dx, smooth: smooth, expanded: this);

            if (executeListeners) {
                ExecuteListeners();
            }
        }

        public void AddListener(System.Action cb) => listeners.Add(cb);

        void ExecuteListeners() => listeners.ForEach(listener => listener?.Invoke());

        public void PositionList(int? dx = null) {
            var listWidth = list.ownTransform.width;
            var unionOffset = 12;
            var menuRightOffset = 8;
            var menuWidth = ownTransform.width + (dx ?? 0);

            list.ownTransform.SetPosition(
                x: (menuWidth - menuRightOffset) - (listWidth - unionOffset)
            );
        }

        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded _) {

            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsToRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));

            PositionList(dx: dx);

            return (dx, dy);
        }

        void InitializeList() {
            list.ClearButtons();
            list.buttons = options.ConvertAll(option => {
                var button = ButtonController.Create(
                    prefab: menuButtonPrefab,
                    parent: list.transform,
                    text: option.text
                );

                button.AddListener(() => {
                    ChangeOption(newOption: option, smooth: true);
                    button.SetState("normal");
                    InputController.instance.ClearFocus();
                });

                return button;
            });
            list.SetButtons();

            PositionList();
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
            InputController.instance.SetFocusParentOnFocusable();

            controller?.GetFocus();

            list.SetVisible(true);

            if (initializeList) {
                initializeList = false;
                InitializeList();
            }
        }

        void IFocusable.LoseFocus() {
            InputController.instance.RemoveFromFocusParent();

            controller?.LoseFocus();
            list.SetVisible(false);
        }

        bool IFocusable.HasFocus() {
            return InputManagment.InputController.instance.handlerWithFocus == (IFocusable)this;
        }

    }
}