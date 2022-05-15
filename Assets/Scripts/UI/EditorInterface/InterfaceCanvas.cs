// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Audio;

namespace ScrapCoder.UI {
    public class InterfaceCanvas : MonoBehaviour {

        // Static variables
        public static InterfaceCanvas instance;

        // Editor variables
        [SerializeField] public NodeTransform editor;
        [SerializeField] public NodeTransform focusParent;

        [SerializeField] public NodeTransform workingZone;
        [SerializeField] public NodeTransform editorControls;
        [SerializeField] public NodeTransform onTopOfEditor;

        [SerializeField] public List<NodeTransform> controls;

        // Lazy variables
        Canvas _canvas;
        public Canvas canvas => _canvas ??= (GetComponent<Canvas>() as Canvas);

        RectTransform _canvasRectTransform;
        public RectTransform canvasRectTransform => _canvasRectTransform ??= (GetComponent<RectTransform>() as RectTransform);

        public new Camera camera => canvas.worldCamera;

        SelectionController _selectionMenus;
        public SelectionController selectionMenus => _selectionMenus ??= (editor.GetComponent<SelectionController>() as SelectionController);

        SoundLibrary _soundLibrary;
        public SoundLibrary soundLibrary => _soundLibrary ??= GetComponent<SoundLibrary>() as SoundLibrary;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Constants
        public const int ScaleFactor = 2;
        public const int NodeScaleFactor = 2;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }

            instance = this;
        }

        public void OnRectTransformDimensionsChange() {
            var dimensions = new Vector2Int {
                x = (int)System.Math.Round(ownTransform.rectTransform.sizeDelta.x),
                y = (int)System.Math.Round(ownTransform.rectTransform.sizeDelta.y)
            };

            editor.ChangeRectTransformDimensions(dimensions.x, dimensions.y);
            editorControls.ChangeRectTransformDimensions(dimensions.x, dimensions.y);
        }

    }
}