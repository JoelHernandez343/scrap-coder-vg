/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerColor {
    Blue,
    Red,
    Green,
    Yellow
}

public enum SetTrigger {
    asParent,
    asChild,
    asLastChild
}

public class NodeTrigger : MonoBehaviour {

    GameObject container;
    List<NodeTrigger> dropZones = new List<NodeTrigger>();

    [SerializeField] public TriggerColor color;

    NodeController3 _controller;
    public NodeController3 controller {
        set {
            _controller = value;
            container = value.container;
        }
        get => _controller;
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        var dropZone = collider.GetComponent<NodeTrigger>();

        if (dropZone != null) {
            dropZones.Add(dropZone);
        }
    }

    public void OnTriggerExit2D(Collider2D collider) {
        var dropZone = collider.GetComponent<NodeTrigger>();

        if (dropZone != null) {
            dropZones.Remove(dropZone);
        }
    }

    public bool OnEndDrag() {
        if (dropZones.Count == 0) {
            return false;
        }

        dropZones.Sort((zoneA, zoneB) => {
            var controllerAIndex = zoneA.controller.transform.GetSiblingIndex();
            var controllerBIndex = zoneB.controller.transform.GetSiblingIndex();

            return controllerAIndex.CompareTo(controllerBIndex);
        });

        return dropZones[dropZones.Count - 1].OnDrop(this);
    }

    protected bool OnDrop(NodeTrigger triggerDropped) {
        return controller.OnDrop(triggerDropped, this);
    }

}
