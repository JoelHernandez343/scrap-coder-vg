// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class Random {
        static System.Random _generator;
        static System.Random generator => _generator ??= new System.Random();

        public static int NextRange(int start, int end) => generator.Next(start, end + 1);

        public static int Next => generator.Next();

        // State variables
        public int seed;

        // Lazy and other variables
        System.Random randGen;

        public int NextInt => randGen.Next();
        public int NextIntRange(int start, int end) => randGen.Next(start, end + 1);

        // Methods
        public Random(int? seed = null) {
            this.seed = seed ?? Random.Next;
            this.randGen = new System.Random(this.seed);
        }
    }
}