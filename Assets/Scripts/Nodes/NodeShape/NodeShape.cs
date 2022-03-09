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
                start = shape.shapePoints[initialStartIndex];
                end = shape.shapePoints[initialEndIndex];

                this.isExpandable = isExpandable;
            }
        }

        // Editor variables
        [SerializeField] SpriteShapeController spriteShapeController;

        [SerializeField] NodeTransform ownTransform;

        // Deprecated
        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        [SerializeField] ShapePointRangeTemplate horizontalRangeTemplate;
        [SerializeField] ShapePointRangeTemplate verticalRangeTemplate;

        [SerializeField] Utils.Vector2D spriteSize = new Utils.Vector2D { x = 8, y = 8 };

        [SerializeField] List<ShapeSegmentTemplate> segmentTemplates;

        // State Variables
        [SerializeField] public List<ShapePoint> shapePoints;

        List<ShapeSegment> segments = new List<ShapeSegment>();

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
        }


        void RenderShape() {
            line.Clear();

            for (var i = 0; i < shapePoints.Count; ++i) {
                var point = shapePoints[i];

                line.InsertPointAt(i, point.position / pixelsPerUnit);
                line.SetSpriteIndex(i, point.spriteIndex);
                line.SetTangentMode(i, ShapeTangentMode.Linear);
            }

            spriteShapeController.RefreshSpriteShape();
        }

        (int dx, int dy) Expand(int dx, int dy, NodeArray _) {
            ownTransform.width += dx;
            ownTransform.height += dy;

            // Width
            if (widthPointsRange.begin > -1) {
                for (var i = widthPointsRange.begin; i <= widthPointsRange.end; ++i) {
                    var point = shapePoints[i];
                    point.position.x += dx;
                    shapePoints[i] = point;

                    line.SetPosition(i, point.position / pixelsPerUnit);
                }
            }

            // Height
            if (heightPointsRange.begin > -1) {
                for (var i = heightPointsRange.begin; i <= heightPointsRange.end; ++i) {
                    var point = shapePoints[i];
                    point.position.y -= dy;
                    shapePoints[i] = point;

                    line.SetPosition(i, point.position / pixelsPerUnit);
                }
            }

            return (dx, dy);
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray _) {
            int[] delta = { dx, dy };

            for (int axis = 0; axis < ranges.Count; ++axis) {
                var range = ranges[axis];
                var isExpandable = range.isExpandable;

                if (!isExpandable) continue;

                shapePoints.ForEach(point => Debug.Log($"BEFORE {point.position.x} {point.position.y}"));

                // for (var pointIndex = shapePoints.IndexOf(range.start); shapePoints[pointIndex - 1] != range.end; ++pointIndex) {
                //     shapePoints[pointIndex].position[axis] += delta[axis];
                // }

                shapePoints.ForEach(point => Debug.Log($"AFTER {point.position.x} {point.position.y}"));
            }
            // ChangeSegments();
            RenderShape();

            return (dx, dy);
        }

        void ChangeSegments() {
            foreach (var segment in segments) {
                var (pointsToAdd, pointsToRemove) = segment.TryChange();

                if (pointsToAdd != null) {
                    var start = shapePoints.IndexOf(segment.realFinalPoint);
                    start += segment.direction == "forward" ? 0 : 1;

                    shapePoints.InsertRange(start, pointsToAdd);
                } else if (pointsToRemove is int removed) {
                    var start = shapePoints.IndexOf(segment.realFinalPoint);
                    start += segment.direction == "forward" ? -removed : 1;

                    shapePoints.RemoveRange(start, removed);
                }
            }
        }
    }
}