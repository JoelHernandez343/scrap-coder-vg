// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SpawnerSelectionButton : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeSprite iconSprite;
        [SerializeField] ExpandableText titleText;

        [SerializeField] SpawnerSelectionController categoryController;

        // State variables
        public string title;
        public string icon;

        public SpawnerSelectionButtonState visibleState = SpawnerSelectionButtonState.HalfVisible;

        // Lazy variables
        ButtonController _button;
        public ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        bool initialized = false;

        // Methods
        void Start() {
            Initialize(title: title, icon: icon);
        }

        public void Initialize(string title, string icon) {

            if (initialized) return;

            titleText.ChangeText(
                newText: title,
                minWidth: 0,
                lettersOffset: 0
            );

            iconSprite.SetState(icon);

            button.AddListener(
                listener: () => SetVisibleState(SpawnerSelectionButtonState.FullVisible),
                eventType: ButtonEventType.OnPointerEnter
            );

            button.AddListener(
                listener: () => categoryController.GetFocus(),
                eventType: ButtonEventType.OnClick
            );

            button.AddListener(
                listener: () => {
                    if (visibleState == SpawnerSelectionButtonState.FullVisible) {
                        SetVisibleState(SpawnerSelectionButtonState.HalfVisible);
                    }
                },
                eventType: ButtonEventType.OnPointerExit
            );

            SetVisibleState(SpawnerSelectionButtonState.HalfVisible);

            initialized = true;
        }

        public void SetVisibleState(SpawnerSelectionButtonState state) {
            if (state == SpawnerSelectionButtonState.FullVisible) {
                ownTransform.SetPosition(x: -10, smooth: true);
            } else if (state == SpawnerSelectionButtonState.HalfVisible) {
                ownTransform.SetPosition(x: -(ownTransform.width - 64), smooth: true);
            } else if (state == SpawnerSelectionButtonState.FullHidden) {
                ownTransform.SetPosition(x: -(ownTransform.width + 10), smooth: true);
            }

            visibleState = state;
        }

    }
}