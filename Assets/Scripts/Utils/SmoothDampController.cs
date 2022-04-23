// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class SmoothDampController {

        Vector2Int currentDelta = Vector2Int.zero;
        Vector2Int destinationDelta = Vector2Int.zero;

        public bool isWorking => !(currentDelta == destinationDelta);
        public bool isFinished => !isWorking;

        public Vector2Int RemainingDelta => destinationDelta - currentDelta;

        float dampingTime;

        Vector2 velocity = Vector2.zero;

        System.Action endingCallback;

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

            if (isFinished) {
                endingCallback = null;
            }
        }

        public (Vector2Int delta, System.Action endingCallback) NextDelta() {
            if (isFinished) {
                var ecb = this.endingCallback;
                Reset();
                return (delta: Vector2Int.zero, endingCallback: ecb);
            }

            var newValue = new Vector2Int();
            var newDelta = Vector2Int.zero;

            for (var axis = 0; axis < 2; ++axis) {
                if (destinationDelta[axis] == currentDelta[axis]) continue;

                var velocity = this.velocity[axis];

                var floatValue = Mathf.SmoothDamp(
                    current: currentDelta[axis],
                    target: destinationDelta[axis],
                    currentVelocity: ref velocity,
                    smoothTime: dampingTime
                );

                this.velocity[axis] = velocity;

                newValue[axis] = currentDelta[axis] < destinationDelta[axis]
                    ? (int)System.Math.Ceiling(floatValue)
                    : (int)System.Math.Floor(floatValue);

                newDelta[axis] = newValue[axis] - currentDelta[axis];
            }

            currentDelta = newValue;

            var endingCallback = isFinished ? this.endingCallback : (System.Action)null;

            Reset(
                resetX: destinationDelta.x == currentDelta.x,
                resetY: destinationDelta.y == currentDelta.y
            );

            return (delta: newDelta, endingCallback: endingCallback);
        }

        public void SetDestination(
            Vector2Int origin,
            int? destinationX = null,
            int? destinationY = null,
            System.Action endingCallback = null,
            bool cancelPreviousCallback = false
        ) {
            if (destinationX == null && destinationY == null) return;

            Reset(
                resetX: destinationX != null,
                resetY: destinationY != null
            );

            var final = new Vector2Int {
                x = destinationX ?? destinationDelta.x,
                y = destinationY ?? destinationDelta.y
            };

            destinationDelta = final - origin;

            if (!cancelPreviousCallback && this.endingCallback != null) {
                this.endingCallback();
            }

            this.endingCallback = endingCallback;

            if (isFinished) {
                Reset();
                if (endingCallback != null) endingCallback();
            }
        }

        public void AddDeltaToDestination(
            int? dx = null,
            int? dy = null,
            System.Action endingCallback = null,
            bool cancelPreviousCallback = false
        ) {
            if (dx == null && dy == null) return;

            if (dx is int Dx) {
                destinationDelta.x += Dx;
            }

            if (dy is int Dy) {
                destinationDelta.y += Dy;
            }

            if (!cancelPreviousCallback && this.endingCallback != null) {
                this.endingCallback();
            }

            this.endingCallback = endingCallback;

            if (isFinished) {
                Reset();
                if (endingCallback != null) endingCallback();
            }
        }
    }
}