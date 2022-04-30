// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {
    public class NodeControllerExpandableByText : MonoBehaviour, INodeExpanded {
        // Editor variables
        [SerializeField] ExpandableText expandableText;
        [SerializeField] NodeTransform pieceToExpand;

        // State variables {serialized so Unity can copy these fields}

        [SerializeField] public string text;

        [SerializeField] public bool hideAfterExpand = false;

        [SerializeField] bool initialized = false;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        NodeController controller => ownTransform.controller;

        bool INodeExpanded.ModifyWidthOfPiece => true;
        bool INodeExpanded.ModifyHeightOfPiece => false;
        NodeTransform INodeExpanded.PieceToExpand => pieceToExpand;

        void Initialize() {
            if (initialized) return;

            ExpandByText();

            if (hideAfterExpand) {
                ownTransform.SetPosition(x: 0, y: 0);
                ownTransform.SetScale(x: 2, y: 2);
                ownTransform.depth = 0;
                HierarchyController.instance.DeleteNode(controller);
            }

            initialized = true;
        }

        void ExpandByText() {
            var dx = expandableText.ChangeText(
                newText: text,
                minWidth: 0,
                lettersOffset: 9
            );

            (controller as INodeExpandable).Expand(
                dx: dx,
                smooth: false,
                expanded: this
            );

            ownTransform.initWidth = ownTransform.width;
            ownTransform.initHeight = ownTransform.height;
        }

        public static NodeController Create(NodeController prefab, Transform parent, string name, string symbolName) {
            var newNode = NodeController.Create(
                prefab: prefab,
                parent: parent,
                template: new NodeControllerTemplate {
                    name = name,
                    symbolName = symbolName
                }
            );

            var newPrefabExpandable = (newNode.GetComponent<NodeControllerExpandableByText>() as NodeControllerExpandableByText);

            newPrefabExpandable.text = name;
            newPrefabExpandable.hideAfterExpand = true;

            newPrefabExpandable.Initialize();

            return newNode;
        }

    }
}