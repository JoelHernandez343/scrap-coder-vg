// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {

    [System.Serializable]
    public class Vector2D {
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

        public Vector2 unityVector => new Vector2 { x = x, y = y };
    }
}