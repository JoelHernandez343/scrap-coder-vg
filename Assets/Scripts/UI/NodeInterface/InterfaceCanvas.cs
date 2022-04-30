// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class InterfaceCanvas : MonoBehaviour {

        // Static variables
        public static InterfaceCanvas instance;

        // Editor variables
        [SerializeField] public NodeTransform nodeInterfaceContainer;
        [SerializeField] public NodeTransform nodeUIContainer;
        [SerializeField] public NodeTransform workingZone;

        // Lazy variables
        Canvas _canvas;
        public Canvas canvas => _canvas ??= (GetComponent<Canvas>() as Canvas);

        RectTransform _canvasRectTransform;
        public RectTransform canvasRectTransform => _canvasRectTransform ??= (GetComponent<RectTransform>() as RectTransform);

        public new Camera camera => canvas.worldCamera;

        // Constants
        public const int OutsideFactor = 2;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }

            instance = this;
        }

    }
}