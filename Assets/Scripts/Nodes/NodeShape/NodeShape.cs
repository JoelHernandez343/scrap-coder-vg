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

        Utils.SmoothDampController smoothDamp = new Utils.SmoothDampController(0.1f);

        bool expandingSmoothly => smoothDamp.isWorking;

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

            RenderShape();
            ChangeSegments();
            RenderShape();
        }

        void FixedUpdate() {
            if (smoothDamp.isWorking) ExpandSmoothly();
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

            smoothDamp.AddDeltaToDestination(
                dx: ranges[0].isExpandable ? dx : (int?)null,
                dy: ranges[1].isExpandable ? dy : (int?)null
            );

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

        void ExpandSmoothly() {
            var (delta, _) = smoothDamp.NextDelta();

            Expand(
                dx: (int)System.Math.Round(delta.x),
                dy: (int)System.Math.Round(delta.y)
            );
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