// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {

    [System.Serializable]
    public struct Vector2D {
        public int x;
        public int y;

        public int this[int index] {
            get {
                if (index == 0) return x;
                if (index == 1) return y;

                throw new System.ArgumentOutOfRangeException($"Does not exists this index: {index}");
            }

            set {
                if (index == 0) {
                    x = value;
                    return;
                }
                if (index == 1) {
                    y = value;
                    return;
                }

                throw new System.ArgumentOutOfRangeException($"Does not exists this index: {index}");
            }
        }

        public (int x, int y) tuple {
            get => (x, y);
            set {
                x = value.x;
                y = value.y;
            }
        }

        public Vector2 unityVector {
            get => new Vector2 { x = x, y = y };
            set {
                x = (int)value.x;
                y = (int)value.y;
            }
        }
    }
}