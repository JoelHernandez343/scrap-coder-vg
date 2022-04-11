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
        [SerializeField] int initialOrder = 0;

        [SerializeField] int publicLastZOrder;

        // State variables
        [SerializeField] List<NodeController> nodes = new List<NodeController>();

        public int? _lastZOrder;
        public int lastDepthOrder {
            get => _lastZOrder ??= initialOrder;
            private set {
                _lastZOrder = value;
                publicLastZOrder = value;
            }
        }

        void Awake() {
            if (instance != null) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        public void SetOnTop(NodeController controller) {
            controller = controller.lastController;
            controller.transform.SetAsLastSibling();

            var index = nodes.IndexOf(controller);

            if (index == -1) {
                nodes.Add(controller);
            } else if (index != nodes.Count - 1) {
                nodes.RemoveAt(index);
                nodes.Add(controller);
            }

            SetSortingOrder();
        }

        public bool Delete(NodeController controller) {
            var index = nodes.IndexOf(controller);

            if (index == -1) return false;

            nodes.RemoveAt(index);

            return true;
        }

        public int IndexOf(NodeController controller) => nodes.IndexOf(controller);

        void SetSortingOrder() {
            for (int i = 0, order = initialOrder, depthOrder = initialOrder; i < nodes.Count; ++i, ++order) {
                if (!nodes[i].hasParent) {
                    var node = nodes[i];

                    node.ownTransform.sorter.sortingOrder = order;
                    node.ownTransform.depth = depthOrder;

                    depthOrder += node.ownTransform.depthLevels;
                    lastDepthOrder = depthOrder;
                }
            }
        }
    }

}