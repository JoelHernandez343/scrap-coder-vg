// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class ShapeSegment {

        // Internal types
        struct GeneratedPoint {
            public int step;
            public int sprite;
        }

        struct GeneratedPair {
            public GeneratedPoint firstPoint;
            public GeneratedPoint finalPoint;
        }

        // State variables
        int normalSprite;
        int rangeSpriteLimit;

        int minSeparation;
        int maxSeparation;

        ShapePoint firstPoint;
        ShapePoint finalPoint;

        Utils.Random random;

        List<GeneratedPair> generatedPairs = new List<GeneratedPair>();

        int lastRenderedPair = -1;

        Utils.Vector2D spriteSize;

        // Lazy and other variables
        int? _axis;
        int axis {
            get {
                _axis ??= firstPoint.position[0] == finalPoint.position[0] ? 1 : 0;
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

        string _direction;
        public string direction {
            get {
                _direction ??= (sign) * firstPoint.position[axis] < (sign) * finalPoint.position[axis]
                    ? "forward"
                    : "backward";

                return _direction;
            }
        }

        public ShapePoint realFirstPoint => direction == "forward" ? firstPoint : finalPoint;
        public ShapePoint realFinalPoint => direction == "forward" ? finalPoint : firstPoint;

        int firstStep => (int)realFirstPoint.position[axis] + (sign) * (int)spriteSize[axis] / 2;

        int prevStep => lastRenderedPair == -1
            ? 0
            : generatedPairs[lastRenderedPair].finalPoint.step + (sign) * (int)spriteSize[axis] / 2;

        int finalStep => (int)realFinalPoint.position[axis] - (sign) * (int)spriteSize[axis];

        // Constructor
        public ShapeSegment(
            NodeShape shape,
            int firstIndex,
            int finalIndex,
            int normalSprite,
            int rangeSpriteLimit,
            Utils.Random random = null,
            Utils.Vector2D spriteSize = null,
            int minSeparation = 6,
            int maxSeparation = 10
        ) {
            firstPoint = shape.shapePoints[firstIndex];
            finalPoint = shape.shapePoints[finalIndex];

            this.spriteSize = spriteSize ?? new Utils.Vector2D { x = 8, y = 8 };
            this.random = random ?? new Utils.Random((new System.Random()).Next());

            this.normalSprite = normalSprite;
            this.rangeSpriteLimit = rangeSpriteLimit;
            this.minSeparation = minSeparation;
            this.maxSeparation = maxSeparation;
        }

        // Methods
        public int GeneratePair() {
            var next = lastRenderedPair + 1;

            if (next < generatedPairs.Count) return next;

            var generatedPosition = random.NextRange(minSeparation, maxSeparation) + prevStep;

            var generatedPair = new GeneratedPair {
                firstPoint = new GeneratedPoint {
                    step = generatedPosition,
                    sprite = random.NextRange(normalSprite, rangeSpriteLimit)
                },
                finalPoint = new GeneratedPoint {
                    step = generatedPosition + (sign) * (int)spriteSize[axis],
                    sprite = normalSprite
                }
            };

            generatedPairs.Add(generatedPair);

            return next;
        }

        public bool CanRenderPair(int candidate) {
            var pair = generatedPairs[candidate];

            return (sign) * (pair.finalPoint.step + firstStep) <= (sign) * finalStep;
        }

        public List<ShapePoint> AddPoints() {
            var pointsToAdd = new List<ShapePoint>();
            var pairs = new List<GeneratedPair>();

            var candidate = GeneratePair();

            while (CanRenderPair(candidate)) {
                lastRenderedPair++;
                pairs.Add(generatedPairs[candidate]);
                pointsToAdd.AddRange(flattenPair(candidate));
                candidate = GeneratePair();
            }

            if (direction == "backward") {
                pointsToAdd.Reverse();
            }

            return pointsToAdd;
        }

        List<ShapePoint> flattenPair(int pairIndex) {
            var pair = generatedPairs[pairIndex];

            return new List<ShapePoint> {
            new ShapePoint {
                position = new Utils.Vector2D {
                    [axis] = firstStep + pair.firstPoint.step,
                    [oppositeAxis] = firstPoint.position[oppositeAxis]
                },
                spriteIndex = pair.firstPoint.sprite
            },
            new ShapePoint {
                position = new Utils.Vector2D {
                    [axis] = firstStep + pair.finalPoint.step,
                    [oppositeAxis] = firstPoint.position[oppositeAxis]
                },
                spriteIndex = pair.finalPoint.sprite
            }
        };
        }

        public int RemovePoints() {
            var pointsToRemove = 0;

            var candidate = lastRenderedPair;

            while (lastRenderedPair != -1 && !CanRenderPair(candidate)) {
                pointsToRemove += 2;
                candidate = lastRenderedPair -= 1;
            }

            return pointsToRemove;
        }

        public (List<ShapePoint> pointsToAdd, int? pointsToRemove) TryChange() {

            var pointsToAdd = AddPoints();
            if (pointsToAdd.Count > 0) return (pointsToAdd, null);

            var pointsToRemove = RemovePoints();
            if (pointsToRemove > 0) return (null, pointsToRemove);

            return (null, null);
        }
    }
}