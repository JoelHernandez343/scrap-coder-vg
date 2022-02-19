// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ScrapCoder.VisualNodes {

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

    public interface IZoneParentRefresher {
        void SetZonesAsParent(NodeArray array);
    }

    public interface INodePartsAdjuster {
        (int dx, int dy) AdjustParts(NodeArray toThisArray, (int dx, int dy)? delta = null);
    }

    public interface INodeSelectorModifier {
        void ModifySelectorFunc();
    }

    public class NodeController : MonoBehaviour {

        public DropFuncSelector selector = new DropFuncSelector();

        [SerializeField] public Canvas canvas;

        [SerializeField] NodeZone topZone;
        [SerializeField] NodeZone middleZone;
        [SerializeField] NodeZone bottomZone;

        [SerializeField] NodeZone lastZone;

        [SerializeField] List<NodeZone> mainZones;


        [SerializeField] public NodeArray siblings;

        [SerializeField] public NodeTransform ownTransform;

        [SerializeField] Component zoneRefresher;
        [SerializeField] Component partsPositioner;
        [SerializeField] Component selectorModifier;

        NodeController _controller;
        public NodeController controller {
            private set {
                _controller = value;

                transform.SetParent(parentArray?.transform ?? canvas.transform);
            }
            get => _controller;
        }

        NodeArray _parentArray;
        public NodeArray parentArray {
            set {
                _parentArray = value;
                controller = parentArray?.controller;
            }
            get => _parentArray;
        }

        public int width => ownTransform.width;
        public int height => ownTransform.height;

        public int x => ownTransform.x;
        public int y => ownTransform.y;
        public int fx => ownTransform.fx;
        public int fy => ownTransform.fy;

        public int initWidth => ownTransform.initWidth;
        public int initHeight => ownTransform.initHeight;

        public (int x, int y) position => ownTransform.position;
        public (int fx, int fy) finalPosition => ownTransform.finalPosition;

        void Awake() {
            selector[ZoneColor.Blue, ZoneColor.Red] = OnBlueThenRed;
            selector[ZoneColor.Red, ZoneColor.Blue] = OnRedThenBlue;
            selector[ZoneColor.Yellow, ZoneColor.Green] = OnYellowThenGreen;

            if (selectorModifier is INodeSelectorModifier modifier) {
                modifier.ModifySelectorFunc();
            }
        }

        public void ClearParent() => parentArray = null;

        public void DetachFromParent() {
            if (controller != null) {
                if (siblings != null) {
                    siblings.AddNodesFromParent();
                } else {
                    parentArray.RemoveNodes(this);
                    RefreshZones();
                    ClearParent();
                }
            }
        }

        public bool HasParent() => parentArray != null;

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
            middleZone?.gameObject.SetActive(enable);
        }

        public void OnBlueThenRed(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            inZone.controller.OnDrop(ownZone, inZone);
        }

        public void OnRedThenBlue(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            siblings.AddNodes(inZone.controller, toThisNode ?? this);
        }

        public void OnYellowThenGreen(NodeZone inZone, NodeZone ownZone, NodeController toThisNode = null) {
            siblings.AddNodes(inZone.controller, toThisNode ?? this);
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

                if (array?.Count == 0) {
                    if (bottomZone != null) {
                        bottomZone.color = ZoneColor.Red;
                    }
                } else {
                    if (bottomZone != null) {
                        bottomZone.color = ZoneColor.Yellow;
                    }
                }
            } else {
                if (zoneRefresher is IZoneParentRefresher refresher) {
                    refresher.SetZonesAsParent(array);
                } else {
                    throw new ArgumentException($"This array {array.gameObject.name} is unknown");
                }
            }
        }

        public void AdjustParts(NodeArray toThisArray, (int dx, int dy) delta) {
            if (toThisArray != siblings) {
                if (partsPositioner is INodePartsAdjuster refresher) {
                    var newDelta = refresher.AdjustParts(toThisArray, delta);

                    ownTransform.Expand(dx: newDelta.dx, dy: newDelta.dy);
                    parentArray?.AdjustParts(this, delta: newDelta);
                } else {
                    throw new System.NotImplementedException("SetPartsPosition method is not implemented");
                }
            }

            if (!HasParent()) {
                HierarchyController.instance.SetOnTop(this);
            }
        }
    }

}