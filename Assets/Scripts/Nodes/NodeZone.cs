// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeZone : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] new BoxCollider2D collider;
        [SerializeField] NodeTransform ownTransform;

        // State variables
        [SerializeField] ZoneColor color;

        List<NodeZone> zones = new List<NodeZone>();

        bool isActive = true;

        // Lazy and other variables
        public ZoneColor zoneColor {
            private set => color = value;
            get => color;
        }

        public NodeController controller => ownTransform.controller;

        // Methods
        public void SetActive(bool isActive) => this.isActive = isActive;

        public void SetZoneColor(ZoneColor color) => this.zoneColor = color;

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

            var validZones = zones.FindAll(zone =>
                zone.isActive &&
                zone.controller.lastController != controller.lastController
            );

            if (validZones.Count == 0) {
                return false;
            }

            validZones.Sort((zoneA, zoneB) => {
                var zA = zoneA.transform.position.z;
                var zB = zoneB.transform.position.z;

                return zA.CompareTo(zB);
            });

            return validZones[0].OnDrop(this);
        }

        public bool OnDrop(NodeZone zone) {
            return controller.OnDrop(zone, this);
        }

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, NodeArray _) {
            var vector = collider.size;

            vector.x += dx;
            vector.y += dy;

            collider.size = vector;

            return (dx, dy);
        }
    }

}
