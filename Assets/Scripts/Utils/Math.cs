// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Utils {
    public class Math {
        static public int mod(int a, int n) {
            return ((a % n) + n) % n;
        }
    }
}
