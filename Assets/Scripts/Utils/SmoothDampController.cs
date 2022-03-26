// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class SmoothDampController {

        Vector2 currentDelta = Vector2.zero;
        Vector2 destinationDelta = Vector2.zero;

        public bool isSmoothing => !(currentDelta == destinationDelta);

        float dampingTime;

        Vector2 velocity = Vector2.zero;

        public SmoothDampController(float dampingTime) {
            this.dampingTime = dampingTime;
        }

        public void Reset(bool resetX = true, bool resetY = true) {
            bool[] reset = { resetX, resetY };

            for (var axis = 0; axis < 2; ++axis) {
                if (!reset[axis]) continue;

                currentDelta[axis] = 0;
                destinationDelta[axis] = 0;

            }
        }

        public Vector2 NextDelta() {
            if (currentDelta == destinationDelta) {
                Reset();
                return Vector2.zero;
            }

            var newValue = new Vector2();
            var newDelta = Vector2.zero;

            for (var axis = 0; axis < 2; ++axis) {
                if (destinationDelta[axis] == currentDelta[axis]) continue;

                var velocity = this.velocity[axis];

                newValue[axis] = Mathf.SmoothDamp(
                    current: currentDelta[axis],
                    target: destinationDelta[axis],
                    currentVelocity: ref velocity,
                    smoothTime: dampingTime
                );

                this.velocity[axis] = velocity;

                newValue[axis] = currentDelta[axis] < destinationDelta[axis]
                    ? (int)System.Math.Ceiling(newValue[axis])
                    : (int)System.Math.Floor(newValue[axis]);

                newDelta[axis] = (int)System.Math.Round(newValue[axis] - currentDelta[axis]);
            }

            currentDelta = newValue;

            Reset(
                resetX: destinationDelta.x == currentDelta.x,
                resetY: destinationDelta.y == currentDelta.y
            );

            return newDelta;
        }

        Vector2 RoundVector(Vector2 vector) => new Vector2 {
            x = (int)System.Math.Round(vector.x),
            y = (int)System.Math.Round(vector.y),
        };

        public void SetDestination(Vector2 origin, int? destinationX, int? destinationY) {
            if (destinationX == null && destinationY == null) return;

            Reset(
                resetX: destinationX != null,
                resetY: destinationY != null
            );

            var final = new Vector2 {
                x = destinationX ?? destinationDelta.x,
                y = destinationY ?? destinationDelta.y
            };

            destinationDelta = RoundVector(final - origin);
        }

        public void AddDeltaToDestination(int? dx, int? dy) {
            if (dx is int Dx) {
                destinationDelta.x += Dx;
            }

            if (dy is int Dy) {
                destinationDelta.y += Dy;
            }
        }
    }
}