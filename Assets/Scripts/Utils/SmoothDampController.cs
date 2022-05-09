// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class SmoothDampController {

        Vector2Int currentDelta = Vector2Int.zero;
        Vector2Int destinationDelta = Vector2Int.zero;

        public bool hasNext => state != "finished";

        public string state
            => (currentDelta != destinationDelta)
                ? "working"
                : (endingCallback != null)
                ? "pendingToFinish"
                : "finished";

        public Vector2Int RemainingDelta => destinationDelta - currentDelta;

        float dampingTime;

        Vector2 velocity = Vector2.zero;

        System.Action endingCallback;

        public SmoothDampController(float dampingTime) {
            this.dampingTime = dampingTime;
        }

        public System.Action Reset(bool resetX = true, bool resetY = false, bool forceResetCallback = false) {
            bool[] reset = { resetX, resetY };

            for (var axis = 0; axis < 2; ++axis) {
                if (!reset[axis]) continue;

                currentDelta[axis] = destinationDelta[axis] = 0;
            }

            return (state == "pendingToFinish" || state == "finished" || forceResetCallback) ? ResetCallback() : null;
        }

        System.Action ResetCallback() {
            System.Action cb;

            (cb, endingCallback) = (endingCallback, null);

            return cb;
        }


        public (Vector2Int delta, System.Action endingCallback) NextDelta() {
            if (state == "pendingToFinish" || state == "finished") {
                return (delta: Vector2Int.zero, endingCallback: Reset());
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

                currentDelta[axis] = newValue[axis];
            }

            var endingCallback = Reset(
                resetX: destinationDelta.x == currentDelta.x,
                resetY: destinationDelta.y == currentDelta.y
            );

            return (delta: newDelta, endingCallback: endingCallback);
        }

        public void SetDeltaDestination(
            int? newDx = null,
            int? newDy = null,
            System.Action endingCallback = null,
            bool executePreviousCallback = false
        ) {
            if (newDx == null && newDy == null) return;

            int?[] newDelta = { newDx, newDy };

            for (var axis = 0; axis < 2; ++axis) {
                if (newDelta[axis] == null) continue;

                destinationDelta[axis] = (int)newDelta[axis];
                currentDelta[axis] = 0;
            }

            if (executePreviousCallback) {
                ResetCallback()?.Invoke();
            }

            this.endingCallback = endingCallback;
        }

        public void AddDeltaToDestination(
            int? dx = null,
            int? dy = null,
            System.Action endingCallback = null,
            bool executePreviousCallback = false
        ) {
            destinationDelta.x += dx ?? 0;
            destinationDelta.y += dy ?? 0;

            if (executePreviousCallback) {
                ResetCallback()?.Invoke();
            }

            this.endingCallback = endingCallback;
        }
    }
}