using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeZoneColor {
    Blue,
    Red,
    Green,
    Yellow,
}

public enum SetZone {
    asParent,
    asChild,
    asLastChild
}

public class NodeZone : MonoBehaviour, INodeExpander {

    [SerializeField] public NodeController controller;
    [SerializeField] new BoxCollider2D collider;
    [SerializeField] public NodeZoneColor color;

    List<NodeZone> zones = new List<NodeZone>();

    public void OnTriggerEnter2D(Collider2D collider) {
        var zone = collider.GetComponent<NodeZone>();

        if (zone?.tag == "TriggerZone") {
            zones.Add(zone);
        }
    }

    public void OnTriggerExit2D(Collider2D collider) {

        var zone = collider.GetComponent<NodeZone>();

        if (zone?.tag == "TriggerZone") {
            zones.Remove(zone);
        }
    }

    public bool Invoke() {
        if (zones.Count == 0) {
            return false;
        }

        zones.Sort((zoneA, zoneB) => {
            var indexA = HierarchyController.instance.IndexOf(zoneA.controller);
            var indexB = HierarchyController.instance.IndexOf(zoneB.controller);

            return indexA.CompareTo(indexB);
        });

        return zones[zones.Count - 1].OnDrop(this);
    }

    public bool OnDrop(NodeZone zone) {
        return controller.OnDrop(zone, this);
    }

    void INodeExpander.Expand(int dx, int dy) {
        var vector = collider.size;

        vector.x += dx;
        vector.y += dy;

        collider.size = vector;
    }
}
