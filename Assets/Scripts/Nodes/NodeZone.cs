// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeZone : MonoBehaviour {

        // State variables
        [SerializeField] ZoneColor color;
        [SerializeField] Component dropHandler;

        List<NodeZone> zones = new List<NodeZone>();

        bool isActive = true;

        // Lazy and other variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

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

            zones = zones.FindAll(zone => zone != null);

            var validZones = zones.FindAll(zone =>
                zone.isActive &&
                zone.controller?.lastController != controller.lastController
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

        public bool HasZone(NodeZone zone) {
            return zones.IndexOf(zone) != -1;
        }

        public bool OnDrop(NodeZone zone) {
            return (this.dropHandler as INodeDropHandler)?.OnDrop(zone, this)
                ?? (controller as INodeDropHandler).OnDrop(zone, this);
        }
    }

}
