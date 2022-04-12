// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ScrapCoder.VisualNodes {

    public interface IZoneParentRefresher {
        void SetZonesAsParent(NodeArray array);
    }

    public interface INodePartsAdjuster {
        (int dx, int dy) AdjustParts(INodeExpandable expandable, (int dx, int dy)? delta = null);
    }

    public interface INodeSelectorModifier {
        void ModifySelectorFunc(NodeController.DropFuncSelector selector);
    }

    public class NodeController : MonoBehaviour {

        // Internal types
        public class DropFuncSelector {
            Func<NodeZone, NodeZone, NodeController, bool> emptyFn = (a, b, c) => false;

            Func<NodeZone, NodeZone, NodeController, bool>[,] funcs =
                new Func<NodeZone, NodeZone, NodeController, bool>[
                    Enum.GetNames(typeof(ZoneColor)).Length,
                    Enum.GetNames(typeof(ZoneColor)).Length
                ];

            public Func<NodeZone, NodeZone, NodeController, bool> this[ZoneColor first, ZoneColor second] {
                get => funcs[(int)first, (int)second] ?? emptyFn;
                set => funcs[(int)first, (int)second] = value;
            }
        }

        // Editor variables
        [SerializeField] public Canvas canvas;

        [SerializeField] NodeZone topZone;
        [SerializeField] NodeZone middleZone;
        [SerializeField] NodeZone bottomZone;

        [SerializeField] NodeZone lastZone;

        [SerializeField] public NodeArray siblings;

        [SerializeField] public NodeType type;
        [SerializeField] public NodeCategory category;

        [SerializeField] List<NodeTransform> components;
        [SerializeField] List<NodeContainer> containers;
        [SerializeField] List<NodeTransform> staticContainers;

        [SerializeField] Component zoneRefresher;
        [SerializeField] Component partsAdjuster;
        [SerializeField] Component selectorModifier;

        // State variables
        NodeArray _parentArray;
        public NodeArray parentArray {
            set {
                _parentArray = value;

                if (value != null) {
                    transform.SetParent(parentArray.transform);
                    HierarchyController.instance.DeleteNode(this);
                } else {
                    transform.SetParent(canvas.transform);
                }
            }
            get => _parentArray;
        }

        [System.NonSerialized] public bool isDragging = false;

        // Lazy and other variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController parentController => parentArray?.controller;

        public NodeController temporalParent {
            set => transform.SetParent(value?.transform ?? canvas.transform);
        }

        DropFuncSelector _selector;
        DropFuncSelector selector {
            get {
                if (_selector == null) {
                    _selector = new DropFuncSelector() {
                        [ZoneColor.Blue, ZoneColor.Red] = AddNodesToIncomingZone,
                        [ZoneColor.Red, ZoneColor.Blue] = AddNodesToContainer,
                        [ZoneColor.Yellow, ZoneColor.Green] = AddNodesToContainer
                    };

                    (selectorModifier as INodeSelectorModifier)?.ModifySelectorFunc(_selector);
                }

                return _selector;
            }
        }

        public bool hasParent => parentArray != null;

        public NodeController lastController {
            get {
                var controller = this;
                while (controller.hasParent) {
                    controller = controller.parentController;
                }

                return controller;
            }
        }

        List<NodeZone> _mainZones;
        public List<NodeZone> mainZones
            => _mainZones ??= (new List<NodeZone> { topZone, middleZone, bottomZone }).FindAll(zone => zone != null);

        List<NodeZone> _validZones;
        public List<NodeZone> validZones
            => _validZones ??= (topZone != null || middleZone != null)
                ? (new List<NodeZone> { topZone, middleZone }).FindAll(zone => zone != null)
                : (new List<NodeZone> { bottomZone }).FindAll(zone => zone != null);

        public UI.DragDropZone previousDrop = null;
        public UI.DragDropZone currentDrop = null;

        // Methods
        public void ClearParent() => parentArray = null;

        public void DetachFromParent() {
            if (!hasParent) return;

            if (siblings != null) {
                siblings.AddNodesFromParent(smooth: true);
            } else {
                parentArray.RemoveNodes(fromThisNode: this, smooth: true);
                RefreshZones();
                ClearParent();
            }
        }

        public void Eject() {
            RefreshZones();
            ClearParent();
        }

        public bool OnDrop(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            if (parentController != null && mainZones.Contains(ownZone)) {
                return parentController.OnDrop(inZone, ownZone, this);
            }

            return selector[ownZone.zoneColor, inZone.zoneColor](inZone, ownZone, toThisNode);
        }

        public bool InvokeZones() {
            return topZone?.Invoke() == true
                || middleZone?.Invoke() == true
                || lastZone?.Invoke() == true;
        }

        void RefreshLastZone() {
            lastZone =
                siblings?.Count == 0
                    ? bottomZone
                    : siblings?.Last.bottomZone;
        }

        public void SetMiddleZone(bool enable) {
            middleZone?.SetActive(enable);
        }

        bool AddNodesToIncomingZone(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            return inZone.controller.OnDrop(ownZone, inZone);
        }

        void AddNodeToContainerDirectly(NodeContainer container, NodeController nodeToAdd) {
            container.AddNodes(nodeToAdd: nodeToAdd, toThisNode: container.Last, smooth: false);
        }

        bool AddNodesToContainer(NodeZone inZone, NodeZone ownZone, NodeController toThisNode) {
            foreach (var container in containers) {
                if (
                    ownZone == container.zone ||
                    ownZone.controller.parentArray == container.array
                ) {
                    container.AddNodes(nodeToAdd: inZone.controller, toThisNode: toThisNode, smooth: true);
                    return true;
                }
            }

            return false;
        }

        public void RefreshZones(NodeArray array = null, NodeController node = null) {
            SetZones(SetZone.asParent, array);
            array?.RefreshNodeZones(node);
            RefreshLastZone();
        }

        public void SetZones(SetZone setting, NodeArray array = null) {
            if (setting == SetZone.asParent) {
                SetZonesAsParent(array);
            } else if (setting == SetZone.asChild) {
                SetZonesAsChild();
            } else if (setting == SetZone.asLastChild) {
                SetZonesAsChild(true);
            }
        }

        void SetZonesAsChild(bool isLast = false) {
            topZone?.SetActive(false);
            bottomZone?.SetZoneColor(isLast ? ZoneColor.Red : ZoneColor.Yellow);
        }

        void SetZonesAsParent(NodeArray array) {
            if (array == siblings || array == null) {
                topZone?.SetActive(true);
                topZone?.SetZoneColor(ZoneColor.Blue);

                SetContainerAsParent(array);
                return;
            }

            if (zoneRefresher is IZoneParentRefresher refresher) {
                refresher.SetZonesAsParent(array);
                return;
            }

            SetContainerAsParent(array);
        }

        void SetContainerAsParent(NodeArray array) {
            if (array == null) return;

            var container = containers.Find(container => container.array == array);

            container.zone.SetZoneColor(array.Count == 0 ? ZoneColor.Red : ZoneColor.Yellow);
        }

        public void AdjustParts(INodeExpandable expandable, (int dx, int dy) delta, bool smooth = false) {

            if ((expandable is NodeContainer container) && container.array == siblings) {
                RefreshLocalDepthLevels();
                HierarchyController.instance.SetOnTopOfNodes(this);
                return;
            }

            var adjuster = partsAdjuster as INodePartsAdjuster;
            var newDelta = adjuster?.AdjustParts(expandable, delta) ?? AdjustPiece(expandable, delta, smooth: smooth);

            ownTransform.Expand(dx: newDelta.dx, dy: newDelta.dy, smooth: smooth);
            RefreshLocalDepthLevels();

            if (hasParent) {
                parentArray.AdjustParts(changedNode: this, dx: newDelta.dx, dy: newDelta.dy, smooth: smooth);
            } else {
                HierarchyController.instance.SetOnTopOfNodes(this);
            }
        }

        (int dx, int dy) AdjustPiece(INodeExpandable expandable, (int dx, int dy) delta, bool smooth = false) {
            var newDelta = delta;

            var pieceToExpand = expandable.PieceToExpand;

            if (pieceToExpand != null) {
                newDelta.dx = expandable.ModifyWidthOfPiece ? newDelta.dx : 0;
                newDelta.dy = expandable.ModifyHeightOfPiece ? newDelta.dy : 0;

                newDelta = pieceToExpand.Expand(dx: newDelta.dx, dy: newDelta.dy, smooth: smooth, expandable: expandable);
            }

            AdjustComponents(pieceModified: pieceToExpand, delta: newDelta, smooth: smooth);

            return newDelta;
        }

        void AdjustComponents(NodeTransform pieceModified, (int dx, int dy) delta, bool smooth = false) {
            var begin = components.IndexOf(pieceModified) + 1;

            for (var i = begin; i < components.Count; ++i) {
                components[i].SetPositionByDelta(dy: -delta.dy, smooth: smooth);
                components[i].Expand(dx: delta.dx, smooth: smooth);
            }
        }

        void RefreshLocalDepthLevels() {
            var localDepthLevels = 0;

            containers.ForEach(container => {
                var tf = container.ownTransform;
                if (localDepthLevels < tf.depthLevels) {
                    localDepthLevels = tf.depthLevels;
                }
            });

            staticContainers.ForEach(container => {
                if (localDepthLevels < container.depthLevels) {
                    localDepthLevels = container.depthLevels;
                }
            });

            ownTransform.localDepthLevels = localDepthLevels;
        }

        public void GetFocus() {
            HierarchyController.instance.SetOnTopOfNodes(parentController ?? this);
            if (hasParent) {
                ownTransform.Raise();

                parentController.GetFocus();
            }
        }

        public void LoseFocus() {
            if (hasParent) {
                ownTransform.ResetRenderOrder();
                parentController.LoseFocus();
            }
        }

        public UI.DragDropZone GetDrop() {

            UI.DragDropZone dropZone = null;

            foreach (var zone in validZones) {

                // Get drop zone of current zone
                var currentDropZone = zone.GetTopDragDropZone();

                // If drop zone is null, return null
                if (currentDropZone == null) return null;

                // Update dropZone only if is null
                dropZone ??= currentDropZone;

                // If there is difference with current drop zone and previous dreop zone, return null
                if (dropZone != currentDropZone) return null;
            }

            return dropZone;
        }

        public void Disappear() {
            Action destroy = () => Destroy(gameObject);

            Action disappear = () => {
                ownTransform.SetPosition(
                    x: ownTransform.x,
                    y: ownTransform.y - 500,
                    smooth: true,
                    endingCallback: destroy
                );
            };

            Action moveUp = () => {
                ownTransform.SetPosition(
                    x: ownTransform.x,
                    y: ownTransform.y - 500,
                    smooth: true,
                    endingCallback: disappear
                );
            };

            ownTransform.SetPosition(
                x: ownTransform.x,
                y: ownTransform.y + 50,
                smooth: true,
                endingCallback: moveUp
            );

            HierarchyController.instance.DeleteNode(this);
        }
    }

}