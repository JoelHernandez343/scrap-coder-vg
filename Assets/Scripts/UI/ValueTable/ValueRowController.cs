// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ValueRowController : MonoBehaviour {

        // Editor variables
        [SerializeField] ExpandableText indexText;
        [SerializeField] ExpandableText valueText;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        public void Initialize(int index, string value) {
            indexText.ChangeText(newText: $"{index}");
            valueText.ChangeText(newText: value);
        }

        public void ChangeValue(string newValue) {
            valueText.ChangeText(newText: newValue);
        }

        public void ChangeIndex(string newIndex) {
            indexText.ChangeText(newText: newIndex);
        }

        public static ValueRowController Create(ValueRowController prefab, Transform parent, int index, string value) {
            var row = Instantiate(original: prefab, parent: parent);

            row.ownTransform.depth = 0;
            row.ownTransform.SetScale(x: 1, y: 1, z: 1);

            row.Initialize(index: index, value: value);

            return row;
        }
    }
}