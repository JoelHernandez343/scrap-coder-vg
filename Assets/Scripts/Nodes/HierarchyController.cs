// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {

    public class HierarchyController : MonoBehaviour {

        // Static variables
        public static HierarchyController instance {
            private set;
            get;
        }

        // Editor variables
        [SerializeField] int initialNodesDepth = 0;
        [SerializeField] int initialNodesOrder = 0;

        [SerializeField] int publicLastNodesDepth;
        [SerializeField] int publicLastNodesOrder;

        // State variables
        [SerializeField] List<NodeController> nodes = new List<NodeController>();

        // Laizy variables
        Transform canvasTransform => InterfaceCanvas.instance.canvas.transform;
        NodeTransform editor => InterfaceCanvas.instance.editor;

        NodeTransform workingZone => InterfaceCanvas.instance.workingZone;
        NodeTransform editorControls => InterfaceCanvas.instance.editorControls;
        NodeTransform onTopOfEditor => InterfaceCanvas.instance.onTopOfEditor;

        NodeTransform focusParent => InterfaceCanvas.instance.focusParent;

        SelectionController selectionMenus => InterfaceCanvas.instance.selectionMenus;
        List<NodeTransform> controls => InterfaceCanvas.instance.controls;

        int? _lastNodesDepth;
        public int lastNodesDepth {
            get => _lastNodesDepth ??= initialNodesDepth;
            private set => _lastNodesDepth = publicLastNodesDepth = value;
        }

        int? _lastNodesOrder;
        public int lastNodesOrder {
            get => _lastNodesOrder ??= initialNodesOrder;
            private set => _lastNodesOrder = publicLastNodesOrder = value;
        }

        int lastEditorControlsDepth => editorControls.depth + editorControls.depthLevels;

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

            controller.transform.SetParent(workingZone.transform);
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

        public void SetOnTopOfEditor(NodeController controller) {
            controller.transform.SetParent(onTopOfEditor.transform);
            controller.ownTransform.sorter.sortingOrder = 2;
            controller.ownTransform.depth = lastEditorControlsDepth + 10;
        }

        public bool DeleteNode(NodeController controller) {
            var index = nodes.IndexOf(controller);

            if (index == -1) return false;

            nodes.RemoveAt(index);

            return true;
        }

        public void SortNodes() {
            var depth = initialNodesDepth;
            var order = initialNodesOrder;

            for (int i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                if (node.hasParent) { Debug.Log("wut"); continue; }

                node.ownTransform.sorter.sortingOrder = order;
                node.ownTransform.depth = depth;

                depth += node.ownTransform.depthLevels;
                order += 1;
            }

            if (depth != lastNodesDepth) {
                var depthDelta = depth - lastNodesDepth;

                lastNodesDepth = depth;
                MoveDepthAboveNodes(depthDelta);
            }

            if (order != lastNodesOrder) {
                var orderDelta = order - lastNodesOrder;

                lastNodesOrder = order;
                MoveOrderAboveNodes(orderDelta);
            }
        }

        void MoveDepthAboveNodes(int delta) {
            editorControls.depth += delta;
            onTopOfEditor.depth += delta;
            focusParent.depth += delta;
        }

        void MoveOrderAboveNodes(int delta) {
            selectionMenus.SetSelectionMenusOrderByDelta(delta);
            controls.ForEach(c => c.sorter.sortingOrder += delta);
            onTopOfEditor.sorter.sortingOrder += delta;
            focusParent.sorter.sortingOrder += delta;
        }

    }

}