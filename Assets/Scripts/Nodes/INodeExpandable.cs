// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public interface INodeExpandable {
        bool ModifyWidthOfPiece { get; }
        bool ModifyHeightOfPiece { get; }

        NodeTransform PieceToExpand { get; }
    }

}