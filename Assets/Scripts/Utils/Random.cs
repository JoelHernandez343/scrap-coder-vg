// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class Random {

        int seed;
        int calls;
        System.Random random;

        public Random(int seed, int calls = 0) {
            random = new System.Random(seed);

            for (var i = 0; i < calls; ++i) {
                random.NextDouble();
            }
        }

        public float Next {
            get {
                calls++;
                return (float)random.NextDouble();
            }
        }

        public int NextRange(int begin, int limit) {
            var generated = Mathf.FloorToInt(Next * (limit + 1 - begin));

            return generated + begin;
        }
    }
}