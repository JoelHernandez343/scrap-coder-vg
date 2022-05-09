// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ButtonController : MonoBehaviour, INodeExpander {

        // Internal types
        class ListenersContainer {
            List<System.Action>[] listenersArray =
                new List<System.Action>[
                    System.Enum.GetNames(typeof(ButtonEventType)).Length
                ];

            public List<System.Action> this[ButtonEventType eventType] {
                get => listenersArray[(int)eventType] ??= new List<System.Action>();
            }
        }

        // Editor variables
        [SerializeField] NodeShapeContainer shapeState;
        [SerializeField] public NodeSprite spriteState;

        [SerializeField] public bool usingSprite = false;

        [SerializeField] List<NodeTransform> itemsToExpand;

        [SerializeField] ExpandableText expandableText;

        // State variable
        [SerializeField] bool activated = true;

        ListenersContainer listenersContainer = new ListenersContainer();

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

        const int lettersOffset = 9;

        // Methods
        void Start() {
            SetActive(activated);
            ownTransform.resizable = !usingSprite;
        }

        public void AddListener(System.Action listener, ButtonEventType eventType = ButtonEventType.OnClick) {
            listenersContainer[eventType].Add(listener);
        }

        public void ClearListeners(ButtonEventType eventType = ButtonEventType.OnClick) {
            listenersContainer[eventType].Clear();
        }

        public void OnTriggerEvent(ButtonEventType eventType = ButtonEventType.OnClick) {
            listenersContainer[eventType].ForEach(listener => listener?.Invoke());
        }

        public int GetListenersCount(ButtonEventType eventType) {
            return listenersContainer[eventType].Count;
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