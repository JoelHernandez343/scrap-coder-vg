// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    [System.Serializable]
    public class ShapeSegmentTemplate {

        public int normalSprite;
        public int rangeSpriteLimit;

        public int minSeparation = 6;
        public int maxSeparation = 10;

        public int firstIndex;
        public int finalIndex;

    }
}