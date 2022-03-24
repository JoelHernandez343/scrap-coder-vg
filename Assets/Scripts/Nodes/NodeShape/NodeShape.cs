// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace ScrapCoder.VisualNodes {

    public class NodeShape : MonoBehaviour, INodeExpander {

        // Internal types
        [System.Serializable]
        struct ShapePointRangeTemplate {
            public int initialStartIndex;
            public int initialEndIndex;
            public bool isExpandable;
        }

        class ShapePointRange {
            public ShapePoint start;
            public ShapePoint end;
            public bool isExpandable;

            public ShapePointRange(NodeShape shape, int initialStartIndex, int initialEndIndex, bool isExpandable) {
                start = shape.points[initialStartIndex];
                end = shape.points[initialEndIndex];

                this.isExpandable = isExpandable;
            }
        }

        // Editor variables
        [SerializeField] SpriteShapeController spriteShapeController;

        [SerializeField] NodeTransform ownTransform;

        [SerializeField] ShapePointRangeTemplate horizontalRangeTemplate;
        [SerializeField] ShapePointRangeTemplate verticalRangeTemplate;

        [SerializeField] Utils.Vector2D spriteSize = new Utils.Vector2D { x = 8, y = 8 };

        [SerializeField] List<ShapeSegmentTemplate> segmentTemplates;

        [SerializeField] Vector2 initialPointPosition;

        // State Variables
        List<ShapeSegment> segments = new List<ShapeSegment>();

        List<ShapePoint> _points;
        public List<ShapePoint> points => _points ??= GetShape();

        ShapePointRange horizontalRange;
        ShapePointRange verticalRange;

        // Lazy and other variables
        List<ShapePointRange> _ranges;
        List<ShapePointRange> ranges {
            get {
                _ranges ??= new List<ShapePointRange> { horizontalRange, verticalRange };

                return _ranges;
            }
        }

        int pixelsPerUnit => NodeTransform.PixelsPerUnit;

        public Spline line => spriteShapeController?.spline;

        bool expandingSmoothly = false;
        float dampingTime = 0.03f;
        Vector2 currentDelta = Vector2.zero;
        Vector2 velocity = Vector2.zero;
        Vector2 destinationDelta = Vector2.zero;

        // Methods
        void Awake() {
            segmentTemplates.ForEach(t => {
                segments.Add(new ShapeSegment(
                    shape: this,
                    firstIndex: t.firstIndex,
                    finalIndex: t.finalIndex,
                    normalSprite: t.normalSprite,
                    rangeSpriteLimit: t.rangeSpriteLimit,
                    spriteSize: spriteSize,
                    minSeparation: t.minSeparation,
                    maxSeparation: t.maxSeparation
                ));
            });

            horizontalRange = new ShapePointRange(
                shape: this,
                initialStartIndex: horizontalRangeTemplate.initialStartIndex,
                initialEndIndex: horizontalRangeTemplate.initialEndIndex,
                isExpandable: horizontalRangeTemplate.isExpandable
            );

            verticalRange = new ShapePointRange(
                shape: this,
                initialStartIndex: verticalRangeTemplate.initialStartIndex,
                initialEndIndex: verticalRangeTemplate.initialEndIndex,
                isExpandable: verticalRangeTemplate.isExpandable
            );

            ChangeSegments();
            RenderShape();
        }

        void Update() {
            if (expandingSmoothly) ExpandSmoothly();
        }

        List<ShapePoint> GetShape() {
            var points = new List<ShapePoint>();
            var original = new List<ShapePoint>();

            for (var i = 0; i < line.GetPointCount(); ++i) {
                var position = line.GetPosition(i);

                original.Add(new ShapePoint {
                    position = new Utils.Vector2D {
                        x = (int)System.Math.Round(position.x * NodeTransform.PixelsPerUnit),
                        y = (int)System.Math.Round(position.y * NodeTransform.PixelsPerUnit),
                    },
                    spriteIndex = line.GetSpriteIndex(i)
                });
            }

            var start = original.FindIndex(point
                => point.position.x == initialPointPosition.x &&
                    point.position.y == initialPointPosition.y
            );

            for (var i = start; i < original.Count; ++i) {
                points.Add(original[i]);
            }

            for (var i = 0; i < start; ++i) {
                points.Add(original[i]);
            }

            return points;
        }

        void RenderShape() {
            line.Clear();

            for (var i = 0; i < points.Count; ++i) {
                var point = points[i];

                line.InsertPointAt(i, point.position.unityVector / pixelsPerUnit);
                line.SetSpriteIndex(i, point.spriteIndex);
                line.SetTangentMode(i, ShapeTangentMode.Linear);
            }

            spriteShapeController.RefreshSpriteShape();
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray _) {

            destinationDelta.x += dx;
            destinationDelta.y += dy;

            expandingSmoothly = true;

            // Expand(dx, dy);

            return (dx, dy);
        }

        void ChangeSegments() {
            foreach (var segment in segments) {
                var (pointsToAdd, pointsToRemove) = segment.TryChange();

                if (pointsToAdd != null) {
                    var start = points.IndexOf(segment.realFinalPoint);
                    start += segment.direction == "forward" ? 0 : 1;

                    points.InsertRange(start, pointsToAdd);
                } else if (pointsToRemove is int removed) {
                    var start = points.IndexOf(segment.realFinalPoint);
                    start += segment.direction == "forward" ? -removed : 1;

                    points.RemoveRange(start, removed);
                }
            }
        }

        void ResetExpandingSmoothly() {
            expandingSmoothly = false;
            currentDelta = destinationDelta = Vector2.zero;
        }

        void ExpandSmoothly() {
            if (this.currentDelta == destinationDelta) {
                ResetExpandingSmoothly();
            }

            var newDelta = Vector2.SmoothDamp(
                current: this.currentDelta,
                target: destinationDelta,
                currentVelocity: ref velocity,
                smoothTime: dampingTime
            );

            newDelta.x = this.currentDelta.x < destinationDelta.x
                ? (int)System.Math.Ceiling(newDelta.x)
                : (int)System.Math.Floor(newDelta.x);
            newDelta.y = this.currentDelta.y < destinationDelta.y
                ? (int)System.Math.Ceiling(newDelta.y)
                : (int)System.Math.Floor(newDelta.y);

            var currentDelta = newDelta - this.currentDelta;

            Expand((int)System.Math.Round(currentDelta.x), (int)System.Math.Round(currentDelta.y));

            this.currentDelta = newDelta;

            if (this.currentDelta == destinationDelta) {
                ResetExpandingSmoothly();
            }
        }

        void Expand(int dx, int dy) {
            if (dx == 0 && dy == 0) {
                return;
            }

            int[] delta = { dx, dy };

            for (int axis = 0; axis < ranges.Count; ++axis) {
                var range = ranges[axis];
                var isExpandable = range.isExpandable;
                var sign = axis == 0 ? 1 : -1;

                if (!isExpandable) continue;

                for (var pointIndex = points.IndexOf(range.start); points[pointIndex - 1] != range.end; ++pointIndex) {
                    points[pointIndex].position[axis] += (sign) * delta[axis];
                }
            }

            ChangeSegments();
            RenderShape();
        }
    }
}