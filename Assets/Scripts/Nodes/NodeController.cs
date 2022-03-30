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
        (int dx, int dy) AdjustParts(NodeArray toThisArray, (int dx, int dy)? delta = null);
    }

    public interface INodeSelectorModifier {
        void ModifySelectorFunc(NodeController.DropFuncSelector selector);
    }

    public class NodeController : MonoBehaviour {

        // Internal types
        public class DropFuncSelector {
            Action<NodeZone, NodeZone, NodeController>[,] funcs =
                new Action<NodeZone, NodeZone, NodeController>[
                    Enum.GetNames(typeof(ZoneColor)).Length,
                    Enum.GetNames(typeof(ZoneColor)).Length
                ];

            public Action<NodeZone, NodeZone, NodeController> this[ZoneColor first, ZoneColor second] {
                get => funcs[(int)first, (int)second];
                set => funcs[(int)first, (int)second] = value;
            }
        }

        // Editor variables
        [SerializeField] public Canvas canvas;

        [SerializeField] NodeZone topZone;
        [SerializeField] NodeZone middleZone;
        [SerializeField] NodeZone bottomZone;

        [SerializeField] NodeZone lastZone;

        [SerializeField] List<NodeZone> mainZones;

        [SerializeField] public NodeArray siblings;

        [SerializeField] public NodeTransform ownTransform;

        [SerializeField] public NodeType type;
        [SerializeField] public NodeCategory category;

        [SerializeField] List<NodeTransform> components;
        [SerializeField] List<NodeContainer> containers;

        [SerializeField] Component zoneRefresher;
        [SerializeField] Component partsAdjuster;
        [SerializeField] Component selectorModifier;

        // State variables
        NodeArray _parentArray;
        public NodeArray parentArray {
            set {
                _parentArray = value;

                transform.SetParent(parentArray?.transform ?? canvas.transform);
            }
            get => _parentArray;
        }

        // Lazy and other variables
        public NodeController controller => parentArray?.controller;

        public NodeController temporalParent {
            set {
                transform.SetParent(value?.transform ?? canvas.transform);
            }
        }

        DropFuncSelector _selector;
        DropFuncSelector selector {
            get {
                if (_selector == null) {
                    _selector = new DropFuncSelector() {
                        [ZoneColor.Blue, ZoneColor.Red] = AddNodesToIncommingZone,
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
                    controller = controller.controller;
                }

                return controller;
            }
        }

        // Methods
        public void ClearParent() => parentArray = null;

        public void DetachFromParent() {
            if (controller == null) return;

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
            if (controller != null && mainZones.Contains(ownZone)) {
                return controller.OnDrop(inZone, ownZone, this);
            }

            var dropFunc = selector[ownZone.zoneColor, inZone.zoneColor];

            if (dropFunc != null) {
                dropFunc(inZone, ownZone, toThisNode);
                return true;
            }

            return false;
        }

        public void InvokeZones() {
            if (topZone?.Invoke() == true) return;
            if (middleZone?.Invoke() == true) return;
            if (lastZone?.Invoke() == true) return;
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

        void AddNodesToIncommingZone(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            inZone.controller.OnDrop(ownZone, inZone);
        }

        void AddNodeToContainerDirectly(NodeContainer container, NodeController nodeToAdd) {
            container.AddNodes(nodeToAdd: nodeToAdd, toThisNode: container.Last, smooth: false);
        }

        void AddNodesToContainer(NodeZone inZone, NodeZone ownZone, NodeController toThisNode) {
            foreach (var container in containers) {
                if (
                    ownZone == container.zone ||
                    ownZone.controller.parentArray == container.array
                ) {
                    container.AddNodes(nodeToAdd: inZone.controller, toThisNode: toThisNode, smooth: true);
                    break;
                }
            }
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

        public void AdjustParts(NodeArray toThisArray, (int dx, int dy) delta, bool smooth = false) {
            if (toThisArray == siblings) {
                RecalculateZLevels();
                HierarchyController.instance.SetOnTop(this);
                return;
            }

            var adjuster = partsAdjuster as INodePartsAdjuster;
            var newDelta = adjuster?.AdjustParts(toThisArray, delta) ?? AdjustPiece(toThisArray, delta, smooth: smooth);

            ownTransform.Expand(dx: newDelta.dx, dy: newDelta.dy, smooth: smooth);
            RecalculateZLevels();

            if (hasParent) {
                parentArray.AdjustParts(changedNode: this, dx: newDelta.dx, dy: newDelta.dy, smooth: smooth);
            } else {
                HierarchyController.instance.SetOnTop(this);
            }
        }

        (int dx, int dy) AdjustPiece(NodeArray array, (int dx, int dy) delta, bool smooth = false) {
            var newDelta = delta;

            var container = containers.Find(container => container.array == array);
            var pieceToExpand = container.pieceToExpand;

            if (pieceToExpand != null) {
                newDelta.dx = container.modifyWidthOfPiece ? newDelta.dx : 0;
                newDelta.dy = container.modifyHeightOfPiece ? newDelta.dy : 0;

                newDelta = pieceToExpand.Expand(dx: newDelta.dx, dy: newDelta.dy, smooth: smooth, fromThisArray: array);
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

        void RecalculateZLevels() {
            var maxZlevels = 0;
            foreach (var container in containers) {
                var tf = container.ownTransform;
                maxZlevels = tf.zLevels < maxZlevels
                    ? tf.zLevels
                    : maxZlevels;
            }

            ownTransform.maxZlevels = maxZlevels;
        }
    }

}