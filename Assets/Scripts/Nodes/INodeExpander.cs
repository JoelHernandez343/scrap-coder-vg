using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeExpander {
        (int dx, int dy) Expand(
            int dx = 0,
            int dy = 0,
            bool smooth = false,
            INodeExpandable expandable = null
        );
    }

}