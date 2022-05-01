// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionCategoryButton : MonoBehaviour {

        // Internal types
        public enum VisibleState { HalfVisible, FullVisible, FullHidden }

        // Editor variables
        [SerializeField] NodeSprite iconSprite;
        [SerializeField] ExpandableText titleText;

        // State variables
        public string title;
        public string icon;

        public VisibleState visibleState = VisibleState.HalfVisible;

        // Lazy variables
        ButtonController _button;
        public ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        bool initialized = false;

        // Methods
        void Start() {
            Initialize();
        }

        public void Initialize() {
            if (initialized) return;

            titleText.ChangeText(
                newText: title,
                minWidth: 0,
                lettersOffset: 0
            );

            iconSprite.SetState(icon);

            button.AddListener(
                listener: () => SetVisibleState(VisibleState.FullVisible),
                eventType: ButtonEventType.OnPointerEnter
            );

            button.AddListener(
                listener: () => SetVisibleState(VisibleState.FullHidden),
                eventType: ButtonEventType.OnClick
            );

            button.AddListener(
                listener: () => {
                    if (visibleState == VisibleState.FullVisible) {
                        SetVisibleState(VisibleState.HalfVisible);
                    }
                },
                eventType: ButtonEventType.OnPointerExit
            );

            initialized = true;
        }

        void SetVisibleState(VisibleState state) {
            if (state == VisibleState.FullVisible) {
                ownTransform.SetPosition(x: -6, smooth: true);
            } else if (state == VisibleState.HalfVisible) {
                ownTransform.SetPosition(x: -112 * InterfaceCanvas.OutsideFactor, smooth: true);
            } else if (state == VisibleState.FullHidden) {
                ownTransform.SetPosition(x: -ownTransform.width * InterfaceCanvas.OutsideFactor - 10, smooth: true);
            }

            visibleState = state;
        }

    }
}