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
        int randomRange;

        int minSeparation;
        int maxSeparation;

        ShapePoint firstPoint;
        ShapePoint finalPoint;

        List<GeneratedPair> generatedPairs = new List<GeneratedPair>();

        int lastRenderedPair = -1;

        Utils.Vector2D spriteSize;

        // Lazy state variables
        Utils.Random _rand;
        Utils.Random rand {
            get => _rand ??= new Utils.Random();
            set => _rand = value;
        }

        // Lazy and other variables
        int? _axis;
        int axis => _axis ??= firstPoint.position[0] == finalPoint.position[0] ? 1 : 0;

        int? _oppositeAxis;
        int oppositeAxis => _oppositeAxis ??= axis == 0 ? 1 : 0;

        int? _sign;
        int sign => _sign ??= axis == 0 ? 1 : -1;

        string _direction;
        public string direction => _direction ??=
            sign * firstPoint.position[axis] < sign * finalPoint.position[axis]
                ? "forward"
                : "backward";

        ShapePoint _realFirstPoint;
        public ShapePoint realFirstPoint
            => _realFirstPoint ??= direction == "forward" ? firstPoint : finalPoint;

        ShapePoint _realFinalPoint;
        public ShapePoint realFinalPoint
            => _realFinalPoint ??= direction == "forward" ? finalPoint : firstPoint;

        int firstStep => realFirstPoint.position[axis] + (sign * spriteSize[axis] / 2);

        int prevStep => lastRenderedPair != -1
            ? generatedPairs[lastRenderedPair].finalPoint.step + (sign * spriteSize[axis] / 2)
            : 0;

        int finalStep => realFinalPoint.position[axis] - (sign * (spriteSize[axis] + 1));

        // Constructor
        public ShapeSegment(ShapeSegmentTemplate template, NodeShape shape) {
            firstPoint = shape.points[template.firstIndex];
            finalPoint = shape.points[template.finalIndex];

            realFirstPoint.segment = this;

            randomRange = template.randomRange;

            minSeparation = template.minSeparation;
            maxSeparation = template.maxSeparation;

            spriteSize = template.spriteSize;

            rand = template.rand;
        }

        // Methods
        public int GeneratePair() {
            var next = lastRenderedPair + 1;

            if (next < generatedPairs.Count) return next;

            var generatedPosition = (sign) * rand.NextIntRange(minSeparation, maxSeparation) + prevStep;

            var generatedPair = new GeneratedPair {
                firstPoint = new GeneratedPoint {
                    step = generatedPosition,
                    sprite = rand.NextIntRange(0, randomRange)
                },
                finalPoint = new GeneratedPoint {
                    step = generatedPosition + (sign) * (int)spriteSize[axis],
                    sprite = 0
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
                    spriteIndex = pair.firstPoint.sprite,
                    segment = this,
                },
                new ShapePoint {
                    position = new Utils.Vector2D {
                        [axis] = firstStep + pair.finalPoint.step,
                        [oppositeAxis] = firstPoint.position[oppositeAxis]
                    },
                    spriteIndex = pair.finalPoint.sprite,
                    segment = this,
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