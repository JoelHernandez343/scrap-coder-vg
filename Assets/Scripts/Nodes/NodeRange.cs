using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {
    [System.Serializable]
    struct NodeRange {
        public int begin;
        public int end;
        public bool isExpandable;
    }
}
