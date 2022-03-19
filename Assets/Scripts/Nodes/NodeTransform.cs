// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeExpander {
        (int dx, int dy) Expand(int dx = 0, int dy = 0, NodeArray fromThisArray = null);
    }

    public class NodeTransform : MonoBehaviour {

        // Editor variables
        [SerializeField] Component nodeExpander;

        [SerializeField] public RectTransform rectTransform;

        [SerializeField] public int initHeight;
        [SerializeField] public int initWidth;

        [SerializeField] int minHeight = 0;
        [SerializeField] int minWidth = 0;

        [SerializeField] bool resizable = true;
        [SerializeField] bool moveable = true;

        [SerializeField] NodeController directController;
        [SerializeField] NodeTransform indirectController;

        [SerializeField] public Utils.Vector2D relativeOrigin;

        [SerializeField] int localZLevels;

        // State Variables
        [System.NonSerialized] public int maxZlevels;

        int? _height;
        public int height {
            set {
                if (!resizable) throw new System.InvalidOperationException("This object is not resizable");

                if (value < minHeight) throw new System.ArgumentException($"Height {value} must be higher than or equal to initHeight: {initHeight}");

                _height = value;
            }

            get {
                _height ??= initHeight;

                return (int)_height;
            }
        }

        int? _width;
        public int width {
            set {
                if (!resizable) throw new System.InvalidOperationException("This object is not resizable");

                if (value < minWidth) throw new System.ArgumentException($"Width {value} must be higher than or equal to initWidth: {initWidth}");

                _width = value;
            }
            get {
                _width ??= initWidth;

                return (int)_width;
            }
        }

        Utils.FloatVector2D floatPosition = new Utils.FloatVector2D();

        // Lazy and other variables
        public Utils.Vector2D position {
            get => new Utils.Vector2D { unityVector = rectTransform.anchoredPosition };
            set => rectTransform.anchoredPosition = value.unityVector;
        }

        UnityEngine.Rendering.SortingGroup _sorter;
        public UnityEngine.Rendering.SortingGroup sorter {
            get {
                _sorter ??= controller.lastController.GetComponent<UnityEngine.Rendering.SortingGroup>();

                return _sorter;
            }
        }

        public int zLevels => localZLevels + maxZlevels;

        public const int PixelsPerUnit = 24;
        public NodeController controller => directController ?? indirectController.controller;

        public int x => position.x;
        public int y => position.y;

        public int fx => x + width;
        public int fy => y - height;

        public (int x, int y) finalPosition => (fx, fy);

        // Methods
        void ResetFloatPosition() {
            floatPosition.tuple = (0, 0);
        }

        void ResetXToRelative() {
            var x = (int)relativeOrigin.x;
            SetPosition(x, y);
        }

        public void ResetYToRelative() {
            var y = (int)relativeOrigin.y;
            SetPosition(x, y);
        }

        void Awake() {
            // width = initWidth;
            // height = initHeight;

            relativeOrigin = new Utils.Vector2D {
                x = position.x,
                y = position.y
            };
        }

        void ChangePosition(int x, int y) {
            position = new Utils.Vector2D { x = x, y = y };
        }

        public void SetPosition((int x, int y) position, bool resetFloatPosition = true) {
            if (!moveable) {
                throw new System.InvalidOperationException("This object is not moveable");
            }

            ChangePosition(position.x, position.y);

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetPosition(int? x = null, int? y = null, bool resetFloatPosition = true) {
            if (!moveable) {
                throw new System.InvalidOperationException("This object is not moveable");
            }

            ChangePosition(x ?? this.x, y ?? this.y);

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetPositionByDelta(int dx = 0, int dy = 0, bool resetFloatPosition = true) {
            if (!moveable) {
                throw new System.InvalidOperationException("This object is not moveable");
            }

            ChangePosition(x + dx, y + dy);

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetFloatPositionByDelta(float dx = 0f, float dy = 0f) {
            floatPosition.x += dx;
            floatPosition.y += dy;

            var intDx = 0;
            var intDy = 0;

            if (floatPosition.x >= 1 || -floatPosition.x >= 1) {
                intDx = floatPosition.intX;

                floatPosition.x -= intDx;
            }
            if (floatPosition.y >= 1 || -floatPosition.y >= 1) {
                intDy = floatPosition.intY;

                floatPosition.y -= intDy;
            }

            SetPositionByDelta(dx: intDx, dy: intDy, resetFloatPosition: false);
        }

        public (int dx, int dy) Expand(int dx = 0, int dy = 0, NodeArray fromThisArray = null) {
            if (!resizable) {
                throw new System.InvalidOperationException("This object is not resizable");
            }

            width += dx;
            height += dy;

            if (nodeExpander is INodeExpander expander) {
                (dx, dy) = expander.Expand(dx, dy, fromThisArray);
            }

            return (dx, dy);
        }

        public void ExpandByNewDimensions(int? newWidth = null, int? newHeight = null) {
            var nW = newWidth ?? width;
            var nH = newHeight ?? height;

            var dx = nW - width;
            var dy = nH - height;

            Expand(dx, dy);
        }

        public void ResetLevelZ() {
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, 0);
        }
    }
}