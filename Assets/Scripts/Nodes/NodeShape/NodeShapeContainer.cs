using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScrapCoder.VisualNodes {

    public class NodeShapeContainer : MonoBehaviour, INodeExpander {
        // Editor variables
        [SerializeField] List<NodeTransform> itemsToExpand;
        [SerializeField] List<NodeTransform> itemsToRight;
        [SerializeField] List<NodeTransform> itemsBelow;

        [SerializeField] NodeShape shape;
        [SerializeField] List<NodeSprite> corners;

        [SerializeField]
        List<string> states = new List<string> {
            "normal",
            "over",
            "pressed",
            "disabled"
        };

        [SerializeField]
        List<NodeShape> fillStates;

        // Methods
        (int? dx, int? dy) INodeExpander.Expand(int? dx, int? dy, bool smooth, INodeExpanded expandable) {

            itemsToExpand.ForEach(i => i.Expand(dx: dx, dy: dy, smooth: smooth));
            itemsToRight.ForEach(i => i.SetPositionByDelta(dx: dx, smooth: smooth));
            itemsBelow.ForEach(i => i.SetPositionByDelta(dy: -dy, smooth: smooth));

            return (dx, dy);
        }

        public void SetState(string state) {
            var stateIndex = states.IndexOf(state);

            if (stateIndex == -1) return;

            shape?.SetState(state);
            corners.ForEach(c => c?.SetState(state));

            for (var i = 0; i < fillStates.Count; ++i) {
                fillStates[i].SetVisible(i == stateIndex);
            }
        }
    }

}