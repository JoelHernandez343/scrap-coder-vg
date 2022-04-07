// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class DropMenuController : MonoBehaviour, INodeExpander {
        // Editor variables
        [SerializeField] List<string> options;
        [SerializeField] ExpandableText text;

        [SerializeField] List<NodeTransform> itemsToExpand;
        [SerializeField] List<NodeTransform> itemsToRight;

        [SerializeField] DropMenuList list;

        // State variables
        string selectedOption = "";

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public List<string> GetOptions() => options;

        // Methods
        void Start() {
            if (options.Count > 0 && selectedOption == "") {
                ChangeOption(options[0]);
            }
        }

        public void ChangeOption(string newOption, bool smooth = false) {
            selectedOption = newOption;

            var delta = text.ChangeText(
                newText: selectedOption,
                minWidth: ownTransform.minWidth,
                lettersOffset: 20
            );

            ownTransform.Expand(dx: delta, smooth: smooth);
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

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, INodeExpandable _) {
            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsToRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));

            PositionList();

            return (dx, dy);
        }
    }
}