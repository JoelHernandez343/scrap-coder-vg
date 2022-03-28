// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class HierarchyController : MonoBehaviour {

        // Editor variables
        [SerializeField] int initialOrder = 0;

        // State variables
        List<NodeController> nodes = new List<NodeController>();

        public int? _lastZOrder;
        public int lastZOrder {
            get => _lastZOrder ??= -initialOrder;
            private set => _lastZOrder = value;
        }

        // Lazy and other variables
        public static HierarchyController instance {
            private set;
            get;
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

        public int IndexOf(NodeController controller) => nodes.IndexOf(controller);

        void SetSortingOrder() {
            for (int i = 0, order = initialOrder, zOrder = -initialOrder; i < nodes.Count; ++i, ++order) {
                if (!nodes[i].hasParent) {
                    var node = nodes[i];

                    var transform = node.ownTransform.rectTransform;
                    var position = transform.localPosition;
                    var sorter = node.ownTransform.sorter;

                    sorter.sortingOrder = order;
                    transform.localPosition = new Vector3(position.x, position.y, zOrder);

                    zOrder += node.ownTransform.zLevels;
                    lastZOrder = zOrder;
                }
            }
        }
    }

}