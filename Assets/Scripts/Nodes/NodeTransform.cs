// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeTransform : MonoBehaviour {

        // Editor variables
        [SerializeField] Component nodeExpander;
        [SerializeField] Component nodeExpandable;

        [SerializeField] NodeController directController;
        [SerializeField] NodeTransform indirectController;

        [SerializeField] public int initHeight;
        [SerializeField] public int initWidth;

        [SerializeField] public int minHeight = 0;
        [SerializeField] public int minWidth = 0;

        [SerializeField] public bool resizable = true;
        [SerializeField] public bool moveable = true;

        [SerializeField] int ownDepthLevels;

        // State Variables
        [System.NonSerialized] public int localDepthLevels;

        int? _height;
        public int height {
            set {
                if (!resizable) throw new System.InvalidOperationException("This object is not resizable");
                if (value < minHeight) throw new System.ArgumentException($"Height {value} must be higher than or equal to initHeight: {initHeight}");

                _height = value;
            }

            get => _height ??= initHeight;
        }

        int? _width;
        public int width {
            set {
                if (!resizable) throw new System.InvalidOperationException("This object is not resizable");
                if (value < minWidth) throw new System.ArgumentException($"Width {value} must be higher than or equal to initWidth: {initWidth}");

                _width = value;
            }
            get => _width ??= initWidth;
        }

        Utils.FloatStack floatX = new Utils.FloatStack { realValue = 0f };
        Utils.FloatStack floatY = new Utils.FloatStack { realValue = 0f };

        // Lazy and other variables<
        RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform ??= GetComponent<RectTransform>();

        public Vector2Int position {
            get => new Vector2Int {
                x = (int)System.Math.Round(rectTransform.anchoredPosition.x),
                y = (int)System.Math.Round(rectTransform.anchoredPosition.y),
            };
            set => rectTransform.anchoredPosition = value;
        }

        UnityEngine.Rendering.SortingGroup _sorter;
        public UnityEngine.Rendering.SortingGroup sorter
            => _sorter ??= GetComponent<UnityEngine.Rendering.SortingGroup>();

        public int depthLevels => ownDepthLevels + localDepthLevels;

        public const int PixelsPerUnit = 24;
        public NodeController controller => directController ?? indirectController?.controller;

        public int x => position.x;
        public int y => position.y;
        public int z {
            get => (int)System.Math.Round(transform.localPosition.z);
            set {
                var pos = transform.localPosition;
                pos.z = value;
                transform.localPosition = pos;
            }
        }

        public int fx => x + width;
        public int fy => y - height;

        public (int x, int y) finalPosition => (fx, fy);

        public int depth {
            get => -z;
            set => z = -value;
        }

        public INodeExpandable expandable => (nodeExpandable as INodeExpandable) ?? controller;

        public bool isMovingSmoothly => smoothDamp.isWorking;

        Utils.SmoothDampController smoothDamp = new Utils.SmoothDampController(0.1f);
        Utils.SmoothDampController smoothDampForDisappearing = new Utils.SmoothDampController(0.1f);

        Vector2Int relativeOrigin;

        // Methods
        void FixedUpdate() {
            if (isMovingSmoothly) {
                MoveSmoothly();
            } else if (smoothDampForDisappearing.isWorking) {
                DisappearSmoothly();
            }
        }

        void MoveSmoothly() {
            var (delta, endingCallback) = smoothDamp.NextDelta();

            position += delta;

            if (!isMovingSmoothly && endingCallback != null) {
                endingCallback();
            }
        }

        void ResetFloatPosition() {
            floatX.realValue = 0f;
            floatY.realValue = 0f;
        }

        public void ResetYToRelative(bool smooth = false) {
            SetPosition(
                x: x,
                y: relativeOrigin.y,
                smooth: smooth
            );
        }

        public void RefreshPosition() {
            MoveToPosition(x: x, y: y);
        }

        void Awake() {
            relativeOrigin = position;
        }

        void MoveToPosition(int? x = null, int? y = null) {
            position = new Vector2Int {
                x = x ?? this.x,
                y = y ?? this.y
            };

            smoothDamp.Reset(
                resetX: x == null,
                resetY: y == null
            );
        }

        public Vector2Int SetPosition(
            int? x = null,
            int? y = null,
            bool resetFloatPosition = true,
            bool smooth = false,
            System.Action endingCallback = null,
            bool cancelPreviousCallback = false
        ) {
            if (!moveable) throw new System.InvalidOperationException("This object is not moveable");

            if (x == null && y == null) return Vector2Int.zero;

            var delta = new Vector2Int { x = x ?? 0, y = y ?? 0 } - position; ;

            if (smooth) {
                smoothDamp.SetDestination(
                    origin: position,
                    destinationX: x,
                    destinationY: y,
                    endingCallback: endingCallback,
                    cancelPreviousCallback: cancelPreviousCallback
                );
            } else {
                MoveToPosition(x, y);
            }

            if (resetFloatPosition) ResetFloatPosition();

            return delta;
        }

        public Vector2Int SetPositionByDelta(
            int? dx = null,
            int? dy = null,
            bool resetFloatPosition = true,
            bool smooth = false,
            System.Action endingCallback = null,
            bool cancelPreviousCallback = false
        ) {
            if (!moveable) throw new System.InvalidOperationException("This object is not moveable");

            if (dx == null && dy == null) return Vector2Int.zero;

            if (smooth) {
                smoothDamp.AddDeltaToDestination(
                    dx: dx,
                    dy: dy,
                    endingCallback: endingCallback,
                    cancelPreviousCallback: cancelPreviousCallback
                );
            } else {
                SetPosition(
                    x: x + dx,
                    y: y + dy,
                    resetFloatPosition: resetFloatPosition
                );
            }

            if (resetFloatPosition) ResetFloatPosition();

            return new Vector2Int { x = dx ?? 0, y = dy ?? 0 };
        }

        public Vector2Int SetFloatPositionByDelta(float? dx = null, float? dy = null, bool smooth = false) {
            float?[] delta = { dx, dy };

            int?[] intDelta = new int?[2];

            Utils.FloatStack[] floatPos = { floatX, floatY };

            for (var axis = 0; axis < 2; ++axis) {
                if (delta[axis] == null) continue;

                floatPos[axis].realValue += delta[axis] ?? 0f;
                intDelta[axis] = floatPos[axis].intValue;

                floatPos[axis].RemoveIntPart();
            }

            return SetPositionByDelta(
                dx: intDelta[0],
                dy: intDelta[1],
                resetFloatPosition: false,
                smooth: smooth
            );
        }

        public (int? dx, int? dy) Expand(int? dx = null, int? dy = null, bool smooth = false, INodeExpanded expanded = null) {
            if (!resizable) {
                throw new System.InvalidOperationException($"[{gameObject.name}] This object is not resizable");
            }

            if (dx == 0 && dy == 0 || dx == null && dy == null) return (dx, dy);

            width += dx ?? 0;
            height += dy ?? 0;

            if (nodeExpander is INodeExpander expander) {
                (dx, dy) = expander.Expand(dx: dx, dy: dy, smooth: smooth, expanded: expanded);
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

        public void ResetRenderOrder(int depthLevels = 0) {
            depth = depthLevels;
            sorter.sortingOrder = 0;
        }

        public void Raise(int deltaOrder = 1, int depthLevels = 1) {
            sorter.sortingOrder += deltaOrder;
            depth += depthLevels;
        }

        public int depthLevelsToThisTransform() {
            if (this.controller == null) return ownDepthLevels;

            var controller = this.controller;
            var depthLevels = controller.ownTransform.ownDepthLevels + ownDepthLevels;
            var array = controller.parentArray;

            while (controller.hasParent) {
                controller = controller.parentController;
                depthLevels +=
                    controller.ownTransform.ownDepthLevels +
                    array.ownTransform.ownDepthLevels +
                    array.container.ownTransform.ownDepthLevels;

                array = controller.parentArray;
            }

            return depthLevels;
        }

        public void Disappear() {
            smoothDampForDisappearing.SetDestination(
                origin: new Vector2Int { x = width, y = 0 },
                destinationX: 0
            );
        }

        public void SetScale(int? x = null, int? y = null, int? z = null) {
            var scale = rectTransform.localScale;

            scale.x = x ?? scale.x;
            scale.y = y ?? scale.y;
            scale.z = z ?? scale.z;

            rectTransform.localScale = scale;
        }

        void DisappearSmoothly() {
            var (delta, _) = smoothDampForDisappearing.NextDelta();
            var percentage = delta / width;

            var scale = rectTransform.localScale;
            scale.x = percentage.x;
            rectTransform.localScale = scale;
        }
    }
}