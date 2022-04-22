using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class FloatStack {

        public float realValue;

        public int intValue
            => realValue >= 0.0
                ? (int)System.Math.Floor(realValue)
                : (int)System.Math.Ceiling(realValue);

        public void RemoveIntPart() {
            realValue -= intValue;
        }

    }
}