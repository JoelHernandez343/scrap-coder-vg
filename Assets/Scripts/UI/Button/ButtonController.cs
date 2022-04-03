// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class ButtonController : MonoBehaviour {

        // Editor variables
        [SerializeField] ButtonCollider buttonCollider;
        [SerializeField] public bool usingSimpleSprites = false;

        // State variable
        [SerializeField] bool activated = true;

        List<System.Action> listeners = new List<System.Action>();

        // Lazy variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        void Start() {
            SetActive(activated);
            ownTransform.resizable = !usingSimpleSprites;
        }

        public void AddListener(System.Action listener) => listeners.Add(listener);

        public bool RemoveListener(System.Action listener) => listeners.Remove(listener);

        public void OnClick() => listeners.ForEach(listener => listener());

        public void SetActive(bool active) {
            activated = active;
            buttonCollider.SetActive(active);
        }
    }
}