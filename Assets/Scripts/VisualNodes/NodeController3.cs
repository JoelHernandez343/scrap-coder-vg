/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NodeController3 : MonoBehaviour {

    protected class DropSelector {
        Action<NodeTrigger, NodeTrigger, NodeController3>[,] funcs =
            new Action<NodeTrigger, NodeTrigger, NodeController3>[
                Enum.GetNames(typeof(TriggerColor)).Length,
                Enum.GetNames(typeof(TriggerColor)).Length
            ];

        public Action<NodeTrigger, NodeTrigger, NodeController3> this[TriggerColor first, TriggerColor second] {
            get => funcs[(int)first, (int)second];
            set => funcs[(int)first, (int)second] = value;
        }
    }

    protected DropSelector dropSelector = new DropSelector();

    protected List<NodeTrigger> ownTriggers;
    protected List<NodeDragger> ownDraggers;
    protected List<NodePart> ownParts;

    protected Canvas mainCanvas;

    protected NodeTrigger triggerTop, triggerBottom, triggerMiddle, lastTrigger;
    protected List<NodeTrigger> mainTriggers = new List<NodeTrigger>();

    public NodeList siblings;
    public GameObject container;
    public RectTransform rectTransform;

    NodeController3 _controller;
    public NodeController3 controller {
        set {
            _controller = value;

            if (controller == null) {
                transform.SetParent(mainCanvas.transform);
            } else {
                transform.SetParent(controller.transform);
            }
        }
        get => _controller;
    }

    NodeList _parentList;
    public NodeList parentList {
        set {
            _parentList = value;
            controller = parentList?.controller;
        }
        get => _parentList;
    }

    public (float x, float y) position {
        get {
            var x = rectTransform.anchoredPosition.x;
            var y = rectTransform.anchoredPosition.y;

            return (x, y);
        }
        private set {
            if (this.position == value) {
                return;
            }

            var position = new Vector2(value.x, value.y);
            rectTransform.anchoredPosition = position;
        }
    }

    [SerializeField] public float height, width;

    public (float x, float y) endPosition => (position.x + width, position.y - height);

    public void Awake() {
        SetComponents();
        ToggleGreenTrigger(false);

        dropSelector[TriggerColor.Red, TriggerColor.Blue] = OnTriggerRedThenBlue;
        dropSelector[TriggerColor.Blue, TriggerColor.Red] = OnTriggerBlueThenRed;
        dropSelector[TriggerColor.Yellow, TriggerColor.Green] = OnTriggerYellowThenGreen;
    }

    public void Start() {
        Init();
    }

    void SetComponents() {
        // Own components
        container = gameObject;
        mainCanvas = transform.parent.GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        siblings = new NodeList(this);

        // Children
        ownTriggers = new List<NodeTrigger>(GetComponentsInChildren<NodeTrigger>());
        ownDraggers = new List<NodeDragger>(GetComponentsInChildren<NodeDragger>());
        ownParts = ownDraggers.ConvertAll(dragger => dragger.GetComponent<NodePart>());

        ownDraggers.ForEach(dragger => dragger.controller = this);
        ownTriggers.ForEach(trigger => trigger.controller = this);
        ownParts.ForEach(part => part.controller = this);

        InitTriggers();
    }

    void InitTriggers() {
        ownTriggers.ForEach(trigger => {
            var name = trigger.gameObject.name;

            if (name == "TopTrigger") {
                triggerTop = trigger;
            } else if (name == "MiddleTrigger") {
                triggerMiddle = trigger;
            } else if (name == "BottomTrigger") {
                triggerBottom = trigger;
                lastTrigger = trigger;
            } else {
                return;
            }

            mainTriggers.Add(trigger);
        });
    }

    void UpdateLastTrigger() {
        lastTrigger =
            siblings.Count == 0
            ? triggerBottom
            : siblings.Last.triggerBottom;
    }

    public void UpdateNodesTriggers(NodeList list) {
        SetTriggers(SetTrigger.asParent, list);

        list.nodes.ForEach(node => {
            if (node == list.Last) {
                node.SetTriggers(SetTrigger.asLastChild);
            } else {
                node.SetTriggers(SetTrigger.asChild);
            }
        });

        UpdateLastTrigger();
    }

    public void ToggleGreenTrigger(bool enable) {
        triggerMiddle?.gameObject.SetActive(enable);
    }

    public void InvokeTriggers() {
        if (triggerTop?.OnEndDrag() == true) return;
        if (triggerMiddle?.OnEndDrag() == true) return;
        if (lastTrigger?.OnEndDrag() == true) return;
    }

    public void DetachFromParent() {
        if (controller == null) {
            return;
        }

        siblings.AddNodesFromParent();
    }

    public void ClearParent() => parentList = null;

    public bool OnDrop(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        if (controller != null && mainTriggers.Find(trigger => trigger == ownTrigger)) {
            return controller.OnDrop(inTrigger, ownTrigger, this);
        }

        var dropFunction = dropSelector[ownTrigger.color, inTrigger.color];

        if (dropFunction != null) {
            dropFunction(inTrigger, ownTrigger, toThisNode);
            return true;
        }

        return false;
    }

    protected virtual void OnTriggerRedThenBlue(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        siblings.AddNodes(inTrigger.controller, toThisNode ?? this);
    }

    protected virtual void OnTriggerBlueThenRed(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        inTrigger.controller.OnDrop(ownTrigger, inTrigger);
    }

    protected virtual void OnTriggerYellowThenGreen(NodeTrigger inTrigger, NodeTrigger ownTrigger, NodeController3 toThisNode = null) {
        siblings.AddNodes(inTrigger.controller, toThisNode ?? this);
    }

    public void SetTriggers(SetTrigger setting, NodeList list = null) {
        if (setting == SetTrigger.asParent) {
            SetTriggersAsParent(list);
        } else if (setting == SetTrigger.asChild) {
            SetTriggersAsChild();
        } else if (setting == SetTrigger.asLastChild) {
            SetTriggersAsLastChild();
        }
    }

    void SetTriggersAsChild() {
        triggerTop?.gameObject.SetActive(false);
        triggerBottom?.gameObject.SetActive(true);

        if (triggerBottom != null) {
            triggerBottom.color = TriggerColor.Yellow;
        }
    }

    void SetTriggersAsLastChild() {
        triggerTop?.gameObject.SetActive(false);
        triggerBottom?.gameObject.SetActive(true);

        if (triggerBottom != null) {
            triggerBottom.color = TriggerColor.Red;
        }
    }

    protected virtual bool SetTriggersAsParent(NodeList list) {
        if (list == siblings) {
            triggerTop?.gameObject.SetActive(true);
            triggerBottom?.gameObject.SetActive(true);

            if (triggerTop != null) {
                triggerTop.color = TriggerColor.Blue;
            }

            if (list.Count == 0) {
                if (triggerBottom != null) {
                    triggerBottom.color = TriggerColor.Red;
                }
            } else {
                if (triggerBottom != null) {
                    triggerBottom.color = TriggerColor.Yellow;
                }
            }
            return true;
        }

        return false;
    }

    public void Refresh(NodeList fromThisList) {
        UpdateNodesTriggers(fromThisList);
    }

    public virtual void SetPartsPosition(NodeController3 node = null) {
        if (node == null) return;

        if (node?.parentList != siblings)
            throw new NotImplementedException("Must be implemented this method if you want use it.");
    }

    public (float x, float y) SetPosition((float x, float y) position) {
        if (this.position != position) {
            this.position = position;
        }

        return endPosition;
    }

    public abstract void SetDimensions();

    public virtual void Init() { }
}
