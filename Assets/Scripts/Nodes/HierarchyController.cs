// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class HierarchyController : MonoBehaviour {

        [SerializeField] int initialOrder = 0;

        List<NodeController> nodes = new List<NodeController>();

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

        public int IndexOf(NodeController controller) {
            return nodes.IndexOf(controller);
        }

        void SetSortingOrder() {
            for (int i = 0, order = initialOrder, zOrder = initialOrder; i < nodes.Count; ++i, ++order) {
                if (!nodes[i].HasParent()) {
                    var node = nodes[i];

                    var sorter = node.GetComponent<UnityEngine.Rendering.SortingGroup>();
                    var transform = node.GetComponent<RectTransform>();
                    var position = transform.localPosition;

                    sorter.sortingOrder = order;
                    transform.localPosition = new Vector3(position.x, position.y, zOrder);

                    zOrder += node.ownTransform.zLevels;
                }
            }
        }
    }

}