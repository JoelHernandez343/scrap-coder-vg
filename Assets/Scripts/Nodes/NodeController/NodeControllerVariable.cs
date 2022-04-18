// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {
    public class NodeControllerVariable : MonoBehaviour, INodeExpanded {

        [SerializeField] ExpandableText expandableText;
        [SerializeField] NodeTransform pieceToExpand;

        [SerializeField] NodeController directController;

        [SerializeField] Transform temporalParent;

        public string text;

        public bool hideAfterExpand = false;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool INodeExpanded.ModifyWidthOfPiece => true;
        bool INodeExpanded.ModifyHeightOfPiece => false;
        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;

        void Start() {
            ExpandByText();

            if (hideAfterExpand) {
                gameObject.transform.SetParent(temporalParent);
                ownTransform.SetPosition(x: 0, y: 0);
                gameObject.SetActive(false);
            }
        }

        void ExpandByText() {
            var dx = expandableText.ChangeText(
                newText: text,
                minWidth: 0,
                lettersOffset: 9
            );

            (directController as INodeExpandable).Expand(
                dx: dx,
                smooth: false,
                expanded: this
            );
        }
    }
}