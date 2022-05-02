// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionCategoryButton : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeSprite iconSprite;
        [SerializeField] ExpandableText titleText;

        // State variables
        public string title;
        public string icon;

        public SelectionCategoryButtonState visibleState = SelectionCategoryButtonState.HalfVisible;

        // Lazy variables
        ButtonController _button;
        public ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        bool initialized = false;

        // Methods
        void Start() {
            Initialize(title: title, icon: icon);
        }

        public void Initialize(string title, string icon, System.Action callback = null) {

            if (initialized) return;

            titleText.ChangeText(
                newText: title,
                minWidth: 0,
                lettersOffset: 0
            );

            iconSprite.SetState(icon);

            button.AddListener(
                listener: () => SetVisibleState(SelectionCategoryButtonState.FullVisible),
                eventType: ButtonEventType.OnPointerEnter
            );

            button.AddListener(
                listener: () => {
                    SetVisibleState(SelectionCategoryButtonState.FullHidden);
                    callback?.Invoke();
                },
                eventType: ButtonEventType.OnClick
            );

            button.AddListener(
                listener: () => {
                    if (visibleState == SelectionCategoryButtonState.FullVisible) {
                        SetVisibleState(SelectionCategoryButtonState.HalfVisible);
                    }
                },
                eventType: ButtonEventType.OnPointerExit
            );

            SetVisibleState(SelectionCategoryButtonState.HalfVisible);

            initialized = true;
        }

        public void SetVisibleState(SelectionCategoryButtonState state) {
            if (state == SelectionCategoryButtonState.FullVisible) {
                ownTransform.SetPosition(x: -6, smooth: true);
            } else if (state == SelectionCategoryButtonState.HalfVisible) {
                ownTransform.SetPosition(x: -112, smooth: true);
            } else if (state == SelectionCategoryButtonState.FullHidden) {
                ownTransform.SetPosition(x: -(ownTransform.width + 10), smooth: true);
            }

            visibleState = state;
        }

    }
}