// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeExpander {
        void Expand(int dx = 0, int dy = 0);
    }

    public class NodeTransform : MonoBehaviour {

        [SerializeField] Component nodeExpander;

        [SerializeField] RectTransform rectTransform;

        [SerializeField] public int initHeight;
        [SerializeField] public int initWidth;

        [SerializeField] bool resizable = true;
        [SerializeField] bool moveable = true;

        public const int PixelsPerUnit = 24;

        [SerializeField] NodeController directController;
        [SerializeField] NodePiece pieceParent;
        public NodeController controller => directController ?? pieceParent.controller;

        int _height;
        public int height {
            set {
                if (!resizable && value != initHeight) {
                    throw new System.InvalidOperationException("This object is not resizable");
                }

                if (value < initHeight) {
                    throw new System.ArgumentException($"Height {value} must be higher than or equal to initHeight: {initHeight}");
                }

                _height = value;
            }

            get => _height;
        }

        int _width;
        public int width {
            set {
                if (!resizable && value != initWidth) {
                    throw new System.InvalidOperationException("This object is not resizable");
                }

                if (value < initWidth) {
                    throw new System.ArgumentException($"Width {value} must be higher than or equal to initWidth: {initWidth}");
                }

                _width = value;
            }
            get => _width;
        }

        public int x => (int)rectTransform.anchoredPosition.x;
        public int y => (int)rectTransform.anchoredPosition.y;

        public (int x, int y) position {
            get => (x, y);
            private set {
                if (this.position == value) {
                    return;
                }

                var position = new Vector2(value.x, value.y);
                rectTransform.anchoredPosition = position;
            }
        }

        public int fx => x + width;
        public int fy => y - height;

        public (int x, int y) finalPosition => (fx, fy);

        (float x, float y) floatPosition;

        void ResetFloatPosition() {
            floatPosition = (0, 0);
        }

        void Awake() {
            width = initWidth;
            height = initHeight;
        }

        public void SetPosition((int x, int y) position, bool resetFloatPosition = true) {
            if (!moveable) {
                throw new System.InvalidOperationException("This object is not moveable");
            }
            this.position = position;

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetPosition(int? x = null, int? y = null, bool resetFloatPosition = true) {
            if (!moveable) {
                throw new System.InvalidOperationException("This object is not moveable");
            }

            var ix = x ?? this.x;
            var iy = y ?? this.y;

            this.position = (ix, iy);

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetPositionByDelta(int dx = 0, int dy = 0, bool resetFloatPosition = true) {
            if (!moveable) {
                throw new System.InvalidOperationException("This object is not moveable");
            }
            position = (x + dx, y + dy);

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetFloatPositionByDelta(float dx = 0f, float dy = 0f) {
            floatPosition.x += dx;
            floatPosition.y += dy;

            var intDx = 0;
            var intDy = 0;

            if (floatPosition.x >= 1 || -floatPosition.x >= 1) {
                intDx = (int)floatPosition.x;

                floatPosition.x -= intDx;
            }
            if (floatPosition.y >= 1 || -floatPosition.y >= 1) {
                intDy = (int)floatPosition.y;

                floatPosition.y -= intDy;
            }

            SetPositionByDelta(dx: intDx, dy: intDy, resetFloatPosition: false);
        }

        public void Expand(int dx = 0, int dy = 0) {
            if (!resizable) {
                throw new System.InvalidOperationException("This object is not resizable");
            }

            width += dx;
            height += dy;

            if (nodeExpander is INodeExpander expander) {
                expander.Expand(dx, dy);
            }
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