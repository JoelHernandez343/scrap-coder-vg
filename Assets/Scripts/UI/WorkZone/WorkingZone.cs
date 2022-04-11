using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class WorkingZone : MonoBehaviour, INodeDropHandler {
        // Static variables
        public static WorkingZone instance { private set; get; }

        // Editor variables
        [SerializeField] Canvas canvas;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        Vector2? _canvasDimensions;
        Vector2 canvasDimensions => _canvasDimensions ??= canvas.GetComponent<RectTransform>().rect.size;

        NodeZone _zone;
        NodeZone zone => _zone ??= (GetComponent<NodeZone>() as NodeZone);

        // Methods
        bool INodeDropHandler.OnDrop(NodeZone incomingZone, NodeZone ownZone) => true;

        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Start() {
            ownTransform.Expand(
                dx: (int)System.Math.Round(canvasDimensions.x - ownTransform.x),
                dy: (int)System.Math.Round(canvasDimensions.y)
            );
        }

        public bool IsOnWorkingZone(NodeController node) {
            var isOnWorkingZone = false;

            foreach (var zone in node.mainZones) {
                if (this.zone.HasZone(zone)) {
                    isOnWorkingZone = true;
                    break;
                }
            }

            return isOnWorkingZone;
        }
    }
}