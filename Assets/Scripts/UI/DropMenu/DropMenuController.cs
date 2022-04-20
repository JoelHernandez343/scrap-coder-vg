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
        [SerializeField] List<string> options;
        [SerializeField] ExpandableText text;

        [SerializeField] List<NodeTransform> itemsToExpand;
        [SerializeField] List<NodeTransform> itemsToRight;

        [SerializeField] public NodeTransform pieceToExpand;

        [SerializeField] DropMenuList list;

        [SerializeField] GameObject removerParent;

        [SerializeField] ButtonController menuButtonPrefab;

        [SerializeField] public bool controlledByExecuter = false;

        // State variables
        string selectedOption = "";

        bool initializeList = true;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController controller => ownTransform.controller;

        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;
        bool INodeExpanded.ModifyHeightOfPiece => false;
        bool INodeExpanded.ModifyWidthOfPiece => true;

        public List<string> GetOptions() => options;

        public string Value => selectedOption;

        // Methods
        void Start() {
            if (options.Count > 0 && selectedOption == "") {
                ChangeOption(options[0]);
            }
        }

        public void ChangeOption(string newOption, bool smooth = false) {
            selectedOption = newOption;

            var dx = text.ChangeText(
                newText: selectedOption,
                minWidth: ownTransform.minWidth,
                lettersOffset: 20
            );

            ownTransform.Expand(dx: dx, smooth: smooth);

            ownTransform.expandable.Expand(dx: dx, smooth: smooth, expanded: this);
        }

        public void PositionList() {
            var listWidth = list.ownTransform.width;
            var unionOffset = 12;
            var menuRightOffset = 8;
            var menuWidth = ownTransform.width;

            list.ownTransform.SetPosition(
                x: (menuWidth - menuRightOffset) - (listWidth - unionOffset)
            );
        }

        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded _) {

            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsToRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));

            PositionList();

            return (dx, dy);
        }

        void InitializeList() {
            list.ClearButtons();
            list.buttons = options.ConvertAll(option => ButtonController.Create(menuButtonPrefab, list.transform, true, option));
            list.SetButtons();
            list.buttons.ForEach(button => button.AddListener(() => {
                // Refresh own text
                ChangeOption(newOption: button.text, smooth: true);

                // Set state of button in normal
                button.SetState("normal");

                // Lose focus
                InputController.instance.ClearFocus();
            }));

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

            var thisDepthLevels = ownTransform.depthLevelsToThisTransform();
            var globalDepthevels = ownTransform.controller?.lastController.ownTransform.depthLevels ?? 0;

            var raiseDepthLevels =
                (globalDepthevels > thisDepthLevels
                    ? globalDepthevels - thisDepthLevels
                    : 0) + HierarchyController.instance.globalRaiseDiff + 10;

            ownTransform.Raise(depthLevels: raiseDepthLevels);

            controller?.GetFocus();
            list.SetVisible(true);

            if (initializeList) {
                initializeList = false;
                InitializeList();
            }
        }

        void IFocusable.LoseFocus() {
            ownTransform.ResetRenderOrder(depthLevels: 1);

            controller?.LoseFocus();
            list.SetVisible(false);
        }

        bool IFocusable.HasFocus() {
            return InputManagment.InputController.instance.handlerWithFocus == (IFocusable)this;
        }
    }
}