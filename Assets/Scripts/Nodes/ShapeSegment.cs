// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    [System.Serializable]
    public class ShapePoint {
        public Vector2 position;
        public int spriteIndex;
    }

    [System.Serializable]
    public class ShapePair {
        public int listIndex;

        public ShapePoint beginPoint;
        public ShapePoint endPoint;

        public List<ShapePoint> flatten {
            get {
                var list = new List<ShapePoint>();

                list.Add(beginPoint);
                list.Add(endPoint);

                return list;
            }
        }
    }

    [System.Serializable]
    public class ShapeSegment {
        [SerializeField] int beginIndex;
        [SerializeField] int endIndex;
        [SerializeField] Vector2 spriteSize;
        [SerializeField] NodeShape shape;

        [SerializeField] int min = 6;
        [SerializeField] int max = 10;

        [SerializeField] int ranSprite;
        [SerializeField] int ranSpriteLimit;

        Vector2 beginPoint => shape.shapePoints[beginIndex].position;
        Vector2 endPoint => shape.shapePoints[endIndex].position;

        Utils.Random _random = null;
        Utils.Random random {
            set {
                _random = value;
            }
            get {
                var rand = new System.Random();
                _random ??= new Utils.Random(rand.Next());

                return _random;
            }
        }

        List<ShapePair> generatedPairs = new List<ShapePair>();
        int lastRenderedPair = -1;

        int? _axis;
        int axis {
            get {
                _axis ??= beginPoint[0] == endPoint[0] ? 1 : 0;
                return (int)_axis;
            }
        }

        int? _oppositeAxis;
        int oppositeAxis {
            get {
                _oppositeAxis ??= axis == 0 ? 1 : 0;
                return (int)_oppositeAxis;
            }
        }

        int? _sign;
        int sign {
            get {
                _sign ??= axis == 0 ? 1 : -1;
                return (int)_sign;
            }
        }

        string _direction = null;
        string direction {
            get {
                _direction ??= (sign) * beginPoint[axis] < (sign) * endPoint[axis]
                    ? "forward"
                    : "backward";

                return _direction;
            }
        }

        int begin {
            get {
                var point = direction == "forward"
                    ? beginPoint
                    : endPoint;

                return (int)point[axis] + (sign) * (int)spriteSize[axis] / 2;
            }
        }

        int prevBegin => lastRenderedPair == -1
            ? begin
            : (int)generatedPairs[lastRenderedPair].endPoint.position[axis] + (sign) * (int)spriteSize[axis] / 2;

        int end {
            get {
                var point = direction == "forward"
                    ? endPoint
                    : beginPoint;

                return (int)point[axis] - (sign) * (int)spriteSize[axis];
            }
        }

        int prevListIndex => lastRenderedPair == -1
            ? beginIndex
            : beginIndex + lastRenderedPair * 2 + 1;

        public void MoveIndexes(int delta) {
            beginIndex += delta;
            endIndex += delta;

            generatedPairs.ForEach(pair => pair.listIndex += delta);
        }

        public List<ShapePoint> AddPoints() {
            var pointsToAdd = new List<ShapePoint>();

            var candidate = GeneratePair();

            while (CanRenderPair(candidate)) {
                lastRenderedPair++;
                pointsToAdd.AddRange(generatedPairs[candidate].flatten);
                candidate = GeneratePair();
            }

            return pointsToAdd;
        }

        public List<ShapePoint> RemovePoints() {
            var pointsToRemove = new List<ShapePoint>();

            var candidate = (int)lastRenderedPair;

            while (lastRenderedPair != -1 || !CanRenderPair(candidate)) {
                lastRenderedPair--;
                pointsToRemove.AddRange(generatedPairs[candidate].flatten);
                candidate = lastRenderedPair -= 1;
            }

            return pointsToRemove;
        }

        public int GeneratePair() {
            var next = lastRenderedPair + 1;

            if (next == generatedPairs.Count) {
                var generated = random.NextRange(prevBegin + min, prevBegin + max);

                var beginPosition = new Vector2();
                beginPosition[axis] = generated;
                beginPosition[oppositeAxis] = beginPoint[oppositeAxis];

                var endPosition = new Vector2();
                endPosition[axis] = generated + (sign) * spriteSize[axis];
                endPosition[oppositeAxis] = beginPoint[oppositeAxis];

                var newPair = new ShapePair {
                    beginPoint = new ShapePoint {
                        position = beginPoint,
                        spriteIndex = random.NextRange(ranSprite, ranSpriteLimit)
                    },
                    endPoint = new ShapePoint {
                        position = endPosition,
                        spriteIndex = ranSprite
                    },
                    listIndex = prevListIndex + 1
                };

                generatedPairs.Add(newPair);
            }

            return next;
        }

        public bool CanRenderPair(int candidate) {
            var point = generatedPairs[candidate];

            return (sign) * point.endPoint.position[axis] < (sign) * endPoint[axis];
        }
    }

}