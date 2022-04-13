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

            public ShapePointRange(ShapePointRangeTemplate template, NodeShape shape) {
                start = shape.points[template.initialStartIndex];
                end = shape.points[template.initialEndIndex];
                isExpandable = template.isExpandable;
            }
        }

        // Editor variables
        [SerializeField] ShapePointRangeTemplate horizontalRangeTemplate;
        [SerializeField] ShapePointRangeTemplate verticalRangeTemplate;

        [SerializeField] Utils.Vector2D spriteSize = new Utils.Vector2D { x = 8, y = 8 };

        [SerializeField] List<ShapeSegmentTemplate> segmentTemplates;

        [SerializeField] Vector2 initialPointPosition;

        [SerializeField] bool hideable = true;

        // State Variables
        List<ShapeSegment> segments = new List<ShapeSegment>();

        List<ShapePoint> _points;
        public List<ShapePoint> points => _points ??= GetShape();

        ShapePointRange horizontalRange;
        ShapePointRange verticalRange;

        // Lazy and other variables
        List<ShapePointRange> _ranges;
        List<ShapePointRange> ranges => _ranges ??= new List<ShapePointRange> { horizontalRange, verticalRange };

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        SpriteShapeController _spriteShapeController;
        SpriteShapeController spriteShapeController => _spriteShapeController ??= GetComponent<SpriteShapeController>();

        SpriteShapeRenderer _spriteShapeRenderer;
        SpriteShapeRenderer spriteShapeRenderer => _spriteShapeRenderer ??= GetComponent<SpriteShapeRenderer>();

        int pixelsPerUnit => NodeTransform.PixelsPerUnit;

        public Spline line => spriteShapeController?.spline;

        Utils.SmoothDampController smoothDamp = new Utils.SmoothDampController(0.1f);

        bool expandingSmoothly => smoothDamp.isWorking;

        // Methods
        void Awake() {
            InitializeSegments();
        }

        void FixedUpdate() {
            if (smoothDamp.isWorking) ExpandSmoothly();
        }

        List<ShapePoint> GetShape() {
            var points = new List<ShapePoint>();
            var original = new List<ShapePoint>();

            var start = 0;

            for (var i = 0; i < line.GetPointCount(); ++i) {
                var position = line.GetPosition(i);

                var newPoint = new ShapePoint {
                    position = new Utils.Vector2D {
                        x = (int)System.Math.Round(position.x * NodeTransform.PixelsPerUnit),
                        y = (int)System.Math.Round(position.y * NodeTransform.PixelsPerUnit),
                    },
                    spriteIndex = line.GetSpriteIndex(i)
                };

                original.Add(newPoint);

                if (newPoint.position.x == initialPointPosition.x &&
                    newPoint.position.y == initialPointPosition.y) {
                    start = i;
                }
            }

            points.AddRange(original.GetRange(start, original.Count - start));
            points.AddRange(original.GetRange(0, start));

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

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, INodeExpandable _) {

            if (smooth) {
                smoothDamp.AddDeltaToDestination(
                    dx: ranges[0].isExpandable ? dx : (int?)null,
                    dy: ranges[1].isExpandable ? dy : (int?)null
                );
            } else {
                Expand(dx: dx, dy: dy);
            }

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

        public void SetVisible(bool visible) {
            if (hideable) {
                spriteShapeRenderer.enabled = visible;
            }
        }

        public void InitializeSegments(int? seed = null) {
            segments.Clear();

            segmentTemplates.ForEach(t => {
                t.spriteSize = this.spriteSize;
                t.rand = new Utils.Random(seed);

                segments.Add(new ShapeSegment(shape: this, template: t));
            });

            CreateRanges();
            ChangeSegments();
            RenderShape();
        }

        void CreateRanges() {
            horizontalRange = new ShapePointRange(shape: this, template: horizontalRangeTemplate);
            verticalRange = new ShapePointRange(shape: this, template: verticalRangeTemplate);
        }
    }
}