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

        [SerializeField] public NodeCategory type;

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
                siblings.AddNodesFromParent();
            } else {
                parentArray.RemoveNodes(this);
                RefreshZones();
                ClearParent();
            }
        }

        public bool OnDrop(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            if (controller != null && mainZones.Contains(ownZone)) {
                return controller.OnDrop(inZone, ownZone, this);
            }

            var dropFunc = selector[ownZone.color, inZone.color];

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

        void AddNodesToContainer(NodeZone inZone, NodeZone ownZone, NodeController toThisNode) {
            foreach (var container in containers) {
                if (
                    ownZone == container.zone ||
                    ownZone.controller.parentArray == container.array
                ) {
                    container.AddNodes(inZone.controller, toThisNode);
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
            topZone?.gameObject.SetActive(false);

            if (bottomZone != null) {
                bottomZone.color = isLast
                    ? ZoneColor.Red
                    : ZoneColor.Yellow;
            }
        }

        void SetZonesAsParent(NodeArray array) {
            if (array == siblings || array == null) {
                topZone?.gameObject.SetActive(true);
                if (topZone != null) {
                    topZone.color = ZoneColor.Blue;
                }

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

            container.zone.color = array.Count == 0
                ? ZoneColor.Red
                : ZoneColor.Yellow;
        }

        public void AdjustParts(NodeArray toThisArray, (int dx, int dy) delta) {
            if (toThisArray == siblings) {
                HierarchyController.instance.SetOnTop(this);
                return;
            }

            var adjuster = partsAdjuster as INodePartsAdjuster;
            var newDelta = adjuster?.AdjustParts(toThisArray, delta) ?? AdjustPiece(toThisArray, delta);

            ownTransform.Expand(dx: newDelta.dx, dy: newDelta.dy);
            RecalculateZLevels();

            if (hasParent) {
                parentArray.AdjustParts(this, delta: newDelta);
            } else {
                HierarchyController.instance.SetOnTop(this);
            }
        }

        (int dx, int dy) AdjustPiece(NodeArray array, (int dx, int dy) delta) {
            var newDelta = delta;

            var container = containers.Find(container => container.array == array);
            var pieceToExpand = container.pieceToExpand;

            if (pieceToExpand != null) {
                newDelta.dx = container.modifyWidthOfPiece ? newDelta.dx : 0;
                newDelta.dy = container.modifyHeightOfPiece ? newDelta.dy : 0;

                newDelta = pieceToExpand.Expand(dx: newDelta.dx, dy: newDelta.dy, array);
            }

            AdjustComponents(pieceToExpand, newDelta);

            return newDelta;
        }

        void AdjustComponents(NodeTransform pieceModified, (int dx, int dy) delta) {
            var begin = components.IndexOf(pieceModified) + 1;

            for (var i = begin; i < components.Count; ++i) {
                components[i].SetPositionByDelta(dy: -delta.dy);
                components[i].Expand(dx: delta.dx);
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