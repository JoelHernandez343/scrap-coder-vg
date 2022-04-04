// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class DropMenuList : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] List<NodeTransform> itemsToExpand;
        [SerializeField] List<NodeTransform> itemsToRight;
        [SerializeField] List<NodeTransform> itemsBelow;

        [SerializeField] Transform buttonsContainer;

        // State variables
        [System.NonSerialized] public List<ButtonController> buttons = new List<ButtonController>();

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        const int buttonSeparation = 4;
        const int buttonMargin = 4;

        // Methods
        void Start() {
            SetButtons();
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, NodeArray _) {

            itemsToExpand.ForEach(item => item.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsToRight.ForEach(item => item.SetPositionByDelta(dx: dx, smooth: smooth));
            itemsBelow.ForEach(item => item.SetPositionByDelta(dy: -dy, smooth: smooth));

            return (dx, dy);
        }

        void SetButtons() {
            var anchor = Vector2Int.zero;
            var newWidth = 0;
            var newHeight = 0;

            buttons.ForEach(button => {
                button.ExpandByText();

                button.transform.SetParent(buttonsContainer);
                button.ownTransform.SetPosition(x: anchor.x, y: anchor.y);

                anchor.y += button.ownTransform.fy - buttonSeparation;

                if (button.ownTransform.width > newWidth) {
                    newWidth = button.ownTransform.width;
                }

                newHeight += button.ownTransform.height + buttonSeparation;
            });

            newWidth += buttonMargin * 2;
            newHeight += buttonMargin * 2;

            ownTransform.ExpandByNewDimensions(newWidth: newWidth, newHeight: newHeight);
        }

        public void SetVisible(bool visible) {
            gameObject.SetActive(visible);
        }

        public static DropMenuList Create(DropMenuList prefab, Transform parent, List<ButtonController> buttons) {
            var dropMenuList = Instantiate(prefab, parent);

            dropMenuList.buttons = buttons;

            return dropMenuList;
        }
    }

}