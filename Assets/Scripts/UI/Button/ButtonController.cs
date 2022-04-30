// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ButtonController : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] NodeShapeContainer shapeState;
        [SerializeField] NodeSprite spriteState;

        [SerializeField] public bool usingSprite = false;

        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] ExpandableText expandableText;

        // State variable
        [SerializeField] bool activated = true;

        List<System.Action> listeners = new List<System.Action>();

        string state;

        // Lazy state variables
        string _text = null;
        public string text {
            get => _text ??= expandableText?.text;
            private set => _text = value;
        }

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public int ListenerCount => listeners.Count;

        const int lettersOffset = 9;

        // Methods
        void Start() {
            SetActive(activated);
            ownTransform.resizable = !usingSprite;
        }

        public void AddListener(System.Action listener) => listeners.Add(listener);

        public bool RemoveListener(System.Action listener) => listeners.Remove(listener);

        public void ClearListeners() => listeners.Clear();

        public void OnClick() {
            listeners.ForEach(listener => listener());
        }

        public void SetActive(bool active) {
            activated = active;
            SetState(activated ? "normal" : "deactivated");
            gameObject.SetActive(activated);
        }

        public void ExpandByText(bool smooth = false) {
            if (expandableText == null) return;

            var delta = expandableText.ChangeText(
                newText: text,
                minWidth: 0,
                lettersOffset: lettersOffset
            );

            expandableText.ownTransform.SetFloatPositionByDelta(dx: -delta / 2.0f);

            ownTransform.Expand(dx: delta, smooth: smooth);
        }

        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded _) {

            itemsToExpand.ForEach(i => i.Expand(dx: dx, dy: dy, smooth: smooth));
            expandableText?.ownTransform.SetFloatPositionByDelta(dx: dx / 2.0f, smooth: smooth);

            return (dx, dy);
        }

        public void SetState(string state) {
            if (this.state == state) return;

            this.state = state;

            if (usingSprite && spriteState != null) {
                spriteState.SetState(state);
            } else if (shapeState != null) {
                shapeState.SetState(state);
            }
        }

        public static ButtonController Create(ButtonController prefab, Transform parent, string text, bool activated = true) {
            var button = Instantiate(prefab, parent);

            button.activated = activated;
            button.text = text;

            // Real dimensions are not here

            return button;
        }
    }
}