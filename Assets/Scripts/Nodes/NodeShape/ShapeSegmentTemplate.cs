// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    [System.Serializable]
    public class ShapeSegmentTemplate {

        public int randomRange = 3;

        public int minSeparation = 6;
        public int maxSeparation = 10;

        public int firstIndex;
        public int finalIndex;

        [System.NonSerialized]
        public Utils.Vector2D spriteSize;

        [System.NonSerialized]
        public Utils.Random rand;

    }
}