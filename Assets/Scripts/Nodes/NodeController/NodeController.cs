// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

using ScrapCoder.Interpreter;
using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {

    public interface IZoneParentRefresher {
        void SetZonesAsParent(NodeArray array);
    }

    public interface INodePartsAdjuster {
        (int dx, int dy) AdjustParts(INodeExpanded expanded, (int dx, int dy)? delta = null);
    }

    public interface INodeSelectorModifier {
        void ModifySelectorFunc(NodeController.DropFuncSelector selector);
    }

    public class NodeController : MonoBehaviour, INodeExpandable {

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
        [SerializeField] NodeZone topZone;
        [SerializeField] NodeZone middleZone;
        [SerializeField] NodeZone bottomZone;

        [SerializeField] NodeZone lastZone;

        [SerializeField] public NodeArray siblings;

        [SerializeField] public NodeType type;
        [SerializeField] public NodeCategory category;

        [SerializeField] List<NodePiece> components;
        [SerializeField] List<NodeContainer> containers;
        [SerializeField] List<NodeTransform> staticContainers;

        [SerializeField] Component zoneRefresher;
        [SerializeField] Component selectorModifier;
        [SerializeField] Component nodeAnalyzer;

        // State variables
        NodeArray _parentArray;
        public NodeArray parentArray {
            set {
                _parentArray = value;

                if (value != null) {
                    transform.SetParent(parentArray.transform);
                    HierarchyController.instance.DeleteNode(this);
                } else {
                    transform.SetParent(workingZone);
                }
            }
            get => _parentArray;
        }

        [System.NonSerialized] public bool isDragging = false;

        public string symbolName;

        // Lazy and other variables
        public Transform workingZone => InterfaceCanvas.instance.workingZone.transform;

        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public NodeController parentController => parentArray?.controller;

        public NodeController temporalParent {
            set => transform.SetParent(value?.transform ?? workingZone);
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

        InterpreterElement _interpreterElement;
        public InterpreterElement interpreterElement => _interpreterElement ??= (GetComponent<InterpreterElement>() as InterpreterElement);

        string state;

        // Methods
        public void ClearParent() => parentArray = null;

        public void DetachFromParent(bool smooth = false) {
            if (!hasParent) return;

            if (siblings != null) {
                siblings.AddNodesFromParent(smooth: smooth);
            } else {
                parentArray.RemoveNodes(fromThisNode: this, smooth: smooth);
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
                    return container.AddNodes(nodeToAdd: inZone.controller, toThisNode: toThisNode, smooth: true);
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

        void INodeExpandable.Expand(int? dx, int? dy, bool smooth, INodeExpanded expanded) {
            if ((expanded is NodeContainer container) && container.array == siblings) {
                RefreshLocalDepthLevels();
                HierarchyController.instance.SetOnTopOfNodes(this);
                return;
            }

            (dx, dy) = AdjustPiece(expanded: expanded, dx: dx, dy: dy, smooth: smooth);

            ownTransform.Expand(dx: dx, dy: dy, smooth: smooth);
            RefreshLocalDepthLevels();

            if (hasParent) {
                parentArray.AdjustParts(changedNode: this, dx: dx, dy: dy, smooth: smooth);
            } else if (transform.parent == workingZone) {
                HierarchyController.instance.SetOnTopOfNodes(this);
            }
        }

        (int? dx, int? dy) AdjustPiece(INodeExpanded expanded, int? dx, int? dy, bool smooth = false) {
            var pieceToExpand = expanded.PieceToExpand;

            if (pieceToExpand != null) {
                dx = expanded.ModifyWidthOfPiece ? dx : null;
                dy = expanded.ModifyHeightOfPiece ? dy : null;

                (dx, dy) = pieceToExpand.Expand(dx: dx, dy: dy, smooth: smooth, expanded: expanded);
            }

            AdjustComponents(pieceModified: pieceToExpand, dx: dx, dy: dy, smooth: smooth);

            return (dx, dy);
        }

        void AdjustComponents(NodeTransform pieceModified, int? dx, int? dy, bool smooth = false) {
            var begin = components.FindIndex(c => c.ownTransform == pieceModified) + 1;

            for (var i = begin; i < components.Count; ++i) {
                components[i].ownTransform.SetPositionByDelta(dy: -dy, smooth: smooth);
                components[i].ownTransform.Expand(dx: dx, smooth: smooth);
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
            if (hasParent) {
                ownTransform.Raise();
                parentController.GetFocus();
            } else {
                HierarchyController.instance.SetOnTopOfNodes(this);
            }
        }

        public void LoseFocus() {
            SetState(state: "normal", propagation: true);

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

            ownTransform.SetPosition(
                x: ownTransform.x,
                y: ownTransform.y + 50,
                smooth: true,
                endingCallback: disappear
            );

            RemoveFromSymbolTable();

            HierarchyController.instance.DeleteNode(this);
            HierarchyController.instance.SortNodes();
        }

        public void RemoveMyself(bool removeChildren) {
            DetachFromParent(smooth: false);

            if (removeChildren) {
                RemoveChildrenFromSymbolTable();
            } else {
                EjectChildren();
            }

            HierarchyController.instance.DeleteNode(this);
            HierarchyController.instance.SortNodes();

            Destroy(gameObject);
        }

        public void RemoveFromSymbolTable() {
            SymbolTable.instance[symbolName]?.RemoveReference(this);
            RemoveChildrenFromSymbolTable();
        }

        void RemoveChildrenFromSymbolTable() {
            containers.ForEach(c => c.RemoveNodesFromTableSymbol());
        }

        void EjectChildren() {
            containers.ForEach(c => c.First?.DetachFromParent(smooth: false));
        }

        public void SetState(string state, bool propagation = false) {
            if (this.state == state) return;

            this.state = state;
            components.ForEach(c => c.SetState(state));

            if (!propagation) return;

            containers.ForEach(c => c.SetState(state));
            parentArray?.SetStateAfterThis(this, state);
        }

        public bool Analyze() {

            foreach (var container in containers) {
                if (container.array == siblings) continue;

                if (container.isEmpty) {
                    Debug.LogError($"This container {container.gameObject.name} is Empty");

                    return false;
                };

                if (!container.Analyze()) return false;
            }

            if (!hasParent) {
                var siblingsContainer = siblings.container;

                if (siblingsContainer.isEmpty) {
                    Debug.LogError($"There must be childs to execute");
                    return false;
                }

                if (!siblingsContainer.Analyze()) return false;

                if (siblingsContainer.Last.type != NodeType.End) {
                    Debug.LogError($"There must be an end connected");
                    return false;
                }
            }

            return (nodeAnalyzer as INodeAnalyzer)?.Analyze() ?? true;
        }

        public Vector2Int BeginDrag(PointerEventData e) {

            SetMiddleZone(true);
            DetachFromParent(smooth: true);

            HierarchyController.instance.SetOnTopOfCanvas(this);

            Vector2Int previousPosition = new Vector2Int { x = ownTransform.x, y = ownTransform.y };

            ownTransform.SetFloatPositionByDelta(
                dx: e.delta.x,
                dy: e.delta.y
            );

            SetState(state: "over", propagation: true);

            return previousPosition;
        }

        public void OnDrag(PointerEventData e) {
            if (e.dragging && isDragging) {
                ownTransform.SetFloatPositionByDelta(
                    dx: e.delta.x,
                    dy: e.delta.y
                );
            }

            currentDrop = GetDrop();

            if (currentDrop != previousDrop) {
                currentDrop?.SetState("over");
                previousDrop?.SetState("normal");

                previousDrop = currentDrop;
            }
        }

        public void OnEndDrag(Vector2Int? previousPosition = null, Action discardCallback = null) {
            var dragDropZone = GetDrop();

            isDragging = false;
            SetState(state: "normal", propagation: true);

            if (dragDropZone?.category == DragDropZone.Category.Working) {

                if (Executer.instance.isRunning || !InvokeZones()) {
                    HierarchyController.instance.SetOnTopOfNodes(this);
                }

                SetMiddleZone(false);
                dragDropZone.SetState("normal");

            } else if (dragDropZone?.category == DragDropZone.Category.Erasing && !Executer.instance.isRunning) {

                Disappear();
                dragDropZone.SetState("normal");

            } else {
                if (discardCallback == null) {
                    ownTransform.SetPosition(
                        x: previousPosition?.x ?? 0,
                        y: previousPosition?.y ?? 0,
                        smooth: true,
                        endingCallback: () => HierarchyController.instance.SetOnTopOfNodes(this)
                    );
                } else {
                    discardCallback();
                }
            }
        }

        public static NodeController Create(NodeController prefab, Transform parent, NodeControllerTemplate template) {
            var newNode = Instantiate(original: prefab, parent: parent);

            newNode.name = template.name;
            newNode.symbolName = template.symbolName;

            newNode.ownTransform.depth = 0;
            newNode.ownTransform.SetScale(x: 1, y: 1, z: 1);

            return newNode;
        }
    }

}