// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeZone : MonoBehaviour {

        // State variables
        [SerializeField] ZoneColor color;

        List<NodeZone> zones = new List<NodeZone>();
        [SerializeField] List<UI.DragDropZone> dropZones = new List<UI.DragDropZone>();

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
            if (collider.gameObject.tag != "TriggerZone") return;

            var zone = (collider.GetComponent<NodeZone>() as NodeZone);

            if (zone != null) {
                zones.Add(zone);
            }

            var dropZone = (collider.GetComponent<UI.DragDropZone>() as UI.DragDropZone);

            if (dropZone != null) {
                dropZones.Add(dropZone);
                dropZones = dropZones.FindAll(zone => zone != null);

                SortList(dropZones);
            }
        }

        public void OnTriggerExit2D(Collider2D collider) {
            if (collider.gameObject.tag != "TriggerZone") return;

            var zone = (collider.GetComponent<NodeZone>() as NodeZone);

            if (zone != null) {
                zones.Remove(zone);
            }

            var dropZone = (collider.GetComponent<UI.DragDropZone>() as UI.DragDropZone);

            if (dropZone != null) {
                dropZones.Remove(dropZone);
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

            SortList(validZones);

            validZones.Sort((zoneA, zoneB) => {
                var zA = zoneA.transform.position.z;
                var zB = zoneB.transform.position.z;

                return zA.CompareTo(zB);
            });

            return validZones[0].OnDrop(this);
        }

        bool OnDrop(NodeZone incomingZone) => controller.OnDrop(inZone: incomingZone, ownZone: this);

        void SortList<T>(List<T> list) where T : MonoBehaviour {
            list.Sort((zoneA, zoneB) => {
                var zA = zoneA.transform.position.z;
                var zB = zoneB.transform.position.z;

                return zA.CompareTo(zB);
            });
        }

        public UI.DragDropZone GetTopDragDropZone() => dropZones.Count > 0 ? dropZones[0] : null;
    }

}
