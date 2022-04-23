// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class GeneralExpander : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] List<NodeTransform> itemsToExpand;
        [SerializeField] List<NodeTransform> itemsToRight;
        [SerializeField] List<NodeTransform> itemsBelow;

        [SerializeField] List<NodeTransform> itemsToCenterHorizontally;
        [SerializeField] List<NodeTransform> itemsToCenterVertically;

        // Methods
        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded expanded) {
            itemsToExpand.ForEach(i => i.Expand(dx: dx, dy: dy, smooth: smooth, expanded: expanded));
            itemsToRight.ForEach(i => i.SetPositionByDelta(dx: dx, smooth: smooth));
            itemsBelow.ForEach(i => i.SetPositionByDelta(dy: -dy, smooth: smooth));

            itemsToCenterHorizontally.ForEach(i => i.SetFloatPositionByDelta(dx: dx / 2f, smooth: smooth));
            itemsToCenterVertically.ForEach(i => i.SetFloatPositionByDelta(dy: -dy / 2f, smooth: smooth));

            return (dx, dy);
        }
    }
}