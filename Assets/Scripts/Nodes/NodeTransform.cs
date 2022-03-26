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

        [SerializeField] public Vector2 relativeOrigin;

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

        Utils.FloatVector2D floatPosition = new Utils.FloatVector2D { x = 0, y = 0 };

        // Lazy and other variables
        public Vector2 position {
            get => new Vector2 {
                x = (int)System.Math.Round(rectTransform.anchoredPosition.x),
                y = (int)System.Math.Round(rectTransform.anchoredPosition.y),
            };
            set => rectTransform.anchoredPosition = value;
        }

        UnityEngine.Rendering.SortingGroup _sorter;
        public UnityEngine.Rendering.SortingGroup sorter
            => _sorter ??= controller.lastController.GetComponent<UnityEngine.Rendering.SortingGroup>();

        public int zLevels => localZLevels + maxZlevels;

        public const int PixelsPerUnit = 24;
        public NodeController controller => directController ?? indirectController.controller;

        public int x => (int)position.x;
        public int y => (int)position.y;

        public int fx => x + width;
        public int fy => y - height;

        public (int x, int y) finalPosition => (fx, fy);

        public bool isMovingSmoothly => smoothDamp.isSmoothing;

        Utils.SmoothDampController smoothDamp = new Utils.SmoothDampController(0.1f);

        System.Action endingCallback;

        // Methods
        void FixedUpdate() {
            if (isMovingSmoothly) MoveSmoothly();
        }

        void MoveSmoothly() {
            if (!isMovingSmoothly) return;

            position += smoothDamp.NextDelta();

            if (!isMovingSmoothly) {
                if (endingCallback != null) {
                    endingCallback();
                    endingCallback = null;
                }
            }
        }

        void ResetFloatPosition() {
            floatPosition.tuple = (0, 0);
        }

        void ResetXToRelative() {
            var x = (int)System.Math.Round(relativeOrigin.x);
            SetPosition(x, y);
        }

        public void ResetYToRelative(bool smooth = false) {
            var y = (int)System.Math.Round(relativeOrigin.y);
            SetPosition(x, y, smooth: smooth);
        }

        public void RefreshPosition() {
            MoveToPosition(x: x, y: y);
        }

        void Awake() {
            relativeOrigin = position;
        }

        void MoveToPosition(int? x = null, int? y = null) {
            position = new Vector2 {
                x = x ?? this.x,
                y = y ?? this.y
            };

            smoothDamp.Reset(
                resetX: x == null,
                resetY: y == null
            );

            endingCallback = null;
        }

        public void SetPosition(int? x = null, int? y = null, bool resetFloatPosition = true, bool smooth = false, System.Action endingCallback = null) {
            if (!moveable) throw new System.InvalidOperationException("This object is not moveable");

            if (x == null && y == null) return;

            if (smooth) {
                smoothDamp.SetDestination(position, x, y);
                this.endingCallback = endingCallback;
            } else {
                MoveToPosition(x, y);
            }

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetPositionByDelta(int? dx = null, int? dy = null, bool resetFloatPosition = true, bool smooth = false, System.Action endingCallback = null) {
            if (!moveable) throw new System.InvalidOperationException("This object is not moveable");

            if (dx == null && dy == null) return;

            if (smooth) {
                smoothDamp.AddDeltaToDestination(dx, dy);
                this.endingCallback = endingCallback;
            } else {
                int?[] change = { x + dx, y + dy };

                SetPosition(
                    x: change[0],
                    y: change[1],
                    resetFloatPosition: resetFloatPosition
                );
            }

            if (resetFloatPosition) ResetFloatPosition();
        }

        public void SetFloatPositionByDelta(float? dx = null, float? dy = null, bool smooth = false) {
            float?[] delta = { dx, dy };

            int?[] intDelta = new int?[2];

            for (var axis = 0; axis < 2; ++axis) {
                if (delta[axis] == null) continue;

                floatPosition[axis] += (float)delta[axis];

                if (floatPosition[axis] >= 1 || -floatPosition[axis] >= 1) {
                    intDelta[axis] = floatPosition.getInt(axis);
                    floatPosition[axis] -= floatPosition.getInt(axis);
                }
            }

            SetPositionByDelta(
                dx: intDelta[0],
                dy: intDelta[1],
                resetFloatPosition: false,
                smooth: smooth
            );
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

        public void ResetRenderOrder() {
            // Reset Z
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, 0);

            // Reset Sorting order
            sorter.sortingOrder = 0;
        }
    }
}