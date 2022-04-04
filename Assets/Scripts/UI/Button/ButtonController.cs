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
        [SerializeField] List<ButtonVisualState> states;

        [SerializeField] ButtonCollider buttonCollider;
        [SerializeField] public bool usingSimpleSprites = false;

        [SerializeField] ExpandableText expandableText;

        // State variable
        [SerializeField] bool activated = true;

        List<System.Action> listeners = new List<System.Action>();

        int? _seed;
        int seed {
            get => _seed ??= Utils.Random.Next;
            set => _seed = value;
        }

        // Lazy state variables
        string _text = null;
        string text {
            get => _text ??= expandableText.text;
            set => _text = value;
        }

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        const int lettersOffset = 8;

        // Methods
        void Start() {
            SetSeed(this.seed);

            SetActive(activated);
            ownTransform.resizable = !usingSimpleSprites;
            ExpandByText(false);
        }

        public void AddListener(System.Action listener) => listeners.Add(listener);

        public bool RemoveListener(System.Action listener) => listeners.Remove(listener);

        public void OnClick() {
            listeners.ForEach(listener => listener());
        }

        public void SetActive(bool active) {
            activated = active;
            buttonCollider.SetActive(active);
        }

        void ExpandByText(bool smooth) {
            var delta = expandableText.ChangeText(
                newText: text,
                minWidth: 0,
                lettersOffset: lettersOffset
            );

            ownTransform.Expand(dx: delta, smooth: smooth);
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, NodeArray _) {

            states.ForEach(item => item.ownTransform.Expand(dx: dx, smooth: smooth));

            expandableText.ownTransform.SetFloatPositionByDelta(dx: dx / 2.0f, smooth: smooth);

            return (dx, dy);
        }

        void SetSeed(int seed) {
            this.seed = seed;
            states.ForEach(state => state.SetSeed(seed));
        }

        public static ButtonController Create(ButtonController prefab, Transform parent, bool activated, string text) {
            var button = Instantiate(prefab, parent);

            button.activated = activated;
            button.text = text;

            // Will expand in Start(), real dimensions are not here

            // Here we need seed

            return button;
        }
    }
}