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

        int? _lastZOrder;
        int lastNodeDepth {
            get => _lastZOrder ??= initialNodeDepth;
            set => _lastZOrder = publicLastZOrder = value;
        }

        void Awake() {
            if (instance != null) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        public void SetOnTopOfNodes(NodeController controller) {
            controller = controller.lastController;
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

        public bool DeleteNode(NodeController controller) {
            var index = nodes.IndexOf(controller);

            if (index == -1) return false;

            nodes.RemoveAt(index);

            return true;
        }

        void SortNodes() {
            for (int i = 0, order = initialNodeDepth, depthOrder = initialNodeDepth; i < nodes.Count; ++i, ++order) {
                if (!nodes[i].hasParent) {
                    var node = nodes[i];

                    node.ownTransform.sorter.sortingOrder = order;
                    node.ownTransform.depth = depthOrder;

                    depthOrder += node.ownTransform.depthLevels;
                    lastNodeDepth = depthOrder;
                }
            }
        }
    }

}