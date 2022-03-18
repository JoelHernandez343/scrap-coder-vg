// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public static class Random {
        private static System.Random _generator;
        private static System.Random generator {
            get {
                _generator ??= new System.Random();

                return _generator;
            }
        }

        public static int NextRange(int start, int end) => generator.Next(start, end + 1);
    }
}