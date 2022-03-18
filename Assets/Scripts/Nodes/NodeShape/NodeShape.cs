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

            ChangeSegments();
            RenderShape();
        }


        void RenderShape() {
            line.Clear();

            for (var i = 0; i < shapePoints.Count; ++i) {
                var point = shapePoints[i];

                line.InsertPointAt(i, point.position.unityVector / pixelsPerUnit);
                line.SetSpriteIndex(i, point.spriteIndex);
                line.SetTangentMode(i, ShapeTangentMode.Linear);
            }

            spriteShapeController.RefreshSpriteShape();
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray _) {
            int[] delta = { dx, dy };

            for (int axis = 0; axis < ranges.Count; ++axis) {
                var range = ranges[axis];
                var isExpandable = range.isExpandable;
                var sign = axis == 0 ? 1 : -1;

                if (!isExpandable) continue;

                for (var pointIndex = shapePoints.IndexOf(range.start); shapePoints[pointIndex - 1] != range.end; ++pointIndex) {
                    shapePoints[pointIndex].position[axis] += (sign) * delta[axis];
                }
            }

            ChangeSegments();
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