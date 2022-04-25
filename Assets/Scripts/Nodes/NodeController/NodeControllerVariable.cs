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

        [SerializeField] public Transform temporalParent;

        public string text;

        public bool hideAfterExpand = false;

        [SerializeField] bool initialized = false;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool INodeExpanded.ModifyWidthOfPiece => true;
        bool INodeExpanded.ModifyHeightOfPiece => false;
        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;

        void Start() {
            if (!initialized) {
                ExpandByText();

                if (hideAfterExpand) {
                    gameObject.transform.SetParent(temporalParent);
                    ownTransform.SetPosition(x: 0, y: 0);
                    ownTransform.SetScale(x: 2, y: 2);
                    ownTransform.depth = 0;
                    HierarchyController.instance.DeleteNode(directController);
                }

                initialized = true;
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

            ownTransform.initWidth = ownTransform.width;
            ownTransform.initHeight = ownTransform.height;
        }
    }
}