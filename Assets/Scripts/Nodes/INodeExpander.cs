// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeExpander {
        (int? dx, int? dy) Expand(
            int? dx = null,
            int? dy = null,
            bool smooth = false,
            INodeExpanded expanded = null
        );
    }

}