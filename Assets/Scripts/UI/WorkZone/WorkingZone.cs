using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class WorkingZone : MonoBehaviour, INodeDropHandler {
        // Editor variables
        [SerializeField] Canvas canvas;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        Vector2? _canvasDimensions;
        Vector2 canvasDimensions => _canvasDimensions ??= canvas.GetComponent<RectTransform>().rect.size;

        bool INodeDropHandler.OnDrop(NodeZone incomingZone, NodeZone ownZone) => true;

        void Start() {
            ownTransform.Expand(
                dx: (int)System.Math.Round(canvasDimensions.x - ownTransform.x),
                dy: (int)System.Math.Round(canvasDimensions.y)
            );
        }
    }
}