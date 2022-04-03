// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScrapCoder.UI {
    public class ButtonController : MonoBehaviour {

        // Editor variables
        [SerializeField] ButtonCollider buttonCollider;

        // State variable
        [SerializeField] bool activated = true;

        List<System.Action> listeners = new List<System.Action>();

        // Methods
        void Start() {
            SetActive(activated);
            Debug.Log("wut");
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