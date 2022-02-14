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
            controller = FindParent(controller);
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

        NodeController FindParent(NodeController controller) {
            while (controller.controller != null) {
                controller = controller.controller;
            }

            return controller;
        }

        void SetSortingOrder() {
            for (int i = 0, order = initialOrder; i < nodes.Count; ++i, ++order) {
                if (!nodes[i].HasParent()) {
                    var sorter = nodes[i].GetComponent<UnityEngine.Rendering.SortingGroup>();
                    var transform = nodes[i].GetComponent<RectTransform>();
                    var position = transform.localPosition;

                    sorter.sortingOrder = order;
                    transform.localPosition = new Vector3(position.x, position.y, -order);
                }
            }
        }
    }

}