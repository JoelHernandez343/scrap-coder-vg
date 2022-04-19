// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class HierarchyController : MonoBehaviour {

        // Static variables
        public static HierarchyController instance {
            private set;
            get;
        }

        // Editor variables
        [SerializeField] int initialNodeDepth = 0;
        [SerializeField] int publicLastZOrder;

        // State variables
        [SerializeField] List<NodeController> nodes = new List<NodeController>();

        [SerializeField] Transform canvas;
        [SerializeField] public Transform workingZone;
        [SerializeField] NodeTransform UIContainer;

        int? _lastNodeDepth;
        int lastNodeDepth {
            get => _lastNodeDepth ??= initialNodeDepth;
            set => _lastNodeDepth = publicLastZOrder = value;
        }

        int initialUIDepth {
            get => UIContainer.depth;
            set => UIContainer.depth = value;
        }
        int lastUIDepth => initialUIDepth + UIContainer.depthLevels;

        public int globalRaiseDiff => lastUIDepth + (initialUIDepth - lastNodeDepth);

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        public void SetOnTopOfNodes(NodeController controller) {
            controller = controller.lastController;

            controller.transform.SetParent(workingZone);
            controller.transform.SetAsLastSibling();

            var index = nodes.IndexOf(controller);

            if (index == -1) {
                nodes.Add(controller);
            } else if (index != nodes.Count - 1) {
                nodes.RemoveAt(index);
                nodes.Add(controller);
            }

            SortNodes();
        }

        public void SetOnTopOfCanvas(NodeController controller) {
            controller.transform.SetParent(canvas);
            controller.ownTransform.sorter.sortingOrder = 2;
            controller.ownTransform.depth = lastUIDepth + 10;
        }

        public bool DeleteNode(NodeController controller) {
            var index = nodes.IndexOf(controller);

            if (index == -1) return false;

            nodes.RemoveAt(index);

            return true;
        }

        public void SortNodes() {
            var depth = initialNodeDepth;

            for (int i = 0, order = initialNodeDepth; i < nodes.Count; ++i, ++order) {
                var node = nodes[i];
                if (node.hasParent) { Debug.Log("wut"); continue; }

                node.ownTransform.sorter.sortingOrder = order;
                node.ownTransform.depth = depth;

                depth += node.ownTransform.depthLevels;
            }

            if (depth != lastNodeDepth) {
                var delta = depth - lastNodeDepth;

                lastNodeDepth = depth;
                MoveUI(delta);
            }
        }

        void MoveUI(int delta) {
            initialUIDepth += delta;
        }
    }

}