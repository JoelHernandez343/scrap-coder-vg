// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343s

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {
    public interface INodeControllerInitializer {

        Dictionary<string, object> GetCustomInfo();
        void Initialize(Dictionary<string, object> customInfo);

    }
}