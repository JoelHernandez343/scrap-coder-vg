// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class Random {

        // Lazy and other variables
        System.Random _random;
        System.Random random {
            get {
                _random ??= new System.Random((new System.Random()).Next());
                return _random;
            }
        }

        public float Next => (float)random.NextDouble();


        public int NextRange(int start, int limit) {
            var generated = Mathf.FloorToInt(Next * (limit + 1 - start));

            return generated + start;
        }
    }
}