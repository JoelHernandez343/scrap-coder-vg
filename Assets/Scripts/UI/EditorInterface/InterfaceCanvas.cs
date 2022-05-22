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
        [SerializeField] public NodeTransform messageParent;

        [SerializeField] public NodeTransform workingZone;
        [SerializeField] public NodeTransform editorControls;
        [SerializeField] public NodeTransform onTopOfEditor;

        [SerializeField] public List<NodeTransform> controls;

        [SerializeField] public ScrollBarController workingZoneVerticalScrollbar;
        [SerializeField] public ScrollBarController workingZoneHorizonalScrollbar;

        // Lazy variables
        Canvas _canvas;
        public Canvas canvas => _canvas ??= (GetComponent<Canvas>() as Canvas);

        RectTransform _canvasRectTransform;
        public RectTransform canvasRectTransform => _canvasRectTransform ??= (GetComponent<RectTransform>() as RectTransform);

        public Camera currentCamera => canvas.worldCamera;

        SelectionController _selectionMenus;
        public SelectionController selectionMenus => _selectionMenus ??= (editor.GetComponent<SelectionController>() as SelectionController);

        public DeclareContainer variableDeclareContainer => selectionMenus.variablesSelection.container.declareContainer;
        public DeclareContainer arrayDeclareContainer => selectionMenus.arraysSelection.container.declareContainer;

        SoundLibrary _soundLibrary;
        public SoundLibrary soundLibrary => _soundLibrary ??= GetComponent<SoundLibrary>() as SoundLibrary;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        Editor _editor;
        public Editor editorVisibiltyManager => _editor ??= (editor.GetComponent<Editor>() as Editor);

        public Vector2Int rectDimensions => new Vector2Int {
            x = (int)System.Math.Round(ownTransform.rectTransform.sizeDelta.x),
            y = (int)System.Math.Round(ownTransform.rectTransform.sizeDelta.y)
        };

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
            var dimensions = rectDimensions;

            editor.ChangeRectTransformDimensions(dimensions.x, dimensions.y);
            editorControls.ChangeRectTransformDimensions(dimensions.x, dimensions.y);

            workingZoneVerticalScrollbar.visor = dimensions.y;
            workingZoneVerticalScrollbar.ownTransform.ExpandByNewDimensions(
                newHeight: dimensions.y / 2 - 36
            );
            workingZoneVerticalScrollbar.RefreshSlider();

            selectionMenus.ExpandContainers(newHeight: (dimensions.y - 153) / 2);
        }

    }
}