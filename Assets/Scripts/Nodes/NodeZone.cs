// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeZone : MonoBehaviour, INodeDropHandler {

        // State variables
        [SerializeField] ZoneColor color;

        List<INodeDropHandler> dropZones = new List<INodeDropHandler>();

        bool isActive = true;

        // Lazy and other variables
        NodeTransform _ownTransform;
        NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        public ZoneColor zoneColor {
            private set => color = value;
            get => color;
        }

        public NodeController controller => ownTransform.controller;

        bool INodeDropHandler.IsActive => isActive;
        Transform INodeDropHandler.Transform => transform;
        NodeTransform INodeDropHandler.OwnTransform => ownTransform;

        // Methods
        public void SetActive(bool isActive) => this.isActive = isActive;

        public void SetZoneColor(ZoneColor color) => this.zoneColor = color;

        public void OnTriggerEnter2D(Collider2D collider) {
            var dropZone = (collider.GetComponent<INodeDropHandler>() as INodeDropHandler);

            if (dropZone != null) {
                dropZones.Add(dropZone);
            }
        }

        public void OnTriggerExit2D(Collider2D collider) {

            var dropZone = (collider.GetComponent<INodeDropHandler>() as INodeDropHandler);

            if (dropZone != null) {
                dropZones.Remove(dropZone);
            }
        }

        public bool Invoke() {
            if (dropZones.Count == 0) {
                return false;
            }

            dropZones = dropZones.FindAll(zone => zone != null);

            var validZones = dropZones.FindAll(dropZone =>
                dropZone.IsActive &&
                dropZone.OwnTransform.controller?.lastController != controller.lastController
            );

            if (validZones.Count == 0) {
                return false;
            }

            validZones.Sort((zoneA, zoneB) => {
                var zA = zoneA.Transform.position.z;
                var zB = zoneB.Transform.position.z;

                return zA.CompareTo(zB);
            });

            return validZones[0].OnDrop(this);
        }

        public bool HasDropZone(INodeDropHandler dropZone) {
            return dropZones.IndexOf(dropZone) != -1;
        }

        bool INodeDropHandler.OnDrop(NodeZone incomingZone) {
            return controller.OnDrop(inZone: incomingZone, ownZone: this);
        }

        public UI.DragDropZone GetFirstDragDropZone() {
            foreach (var dropZone in dropZones) {
                if (dropZone is UI.DragDropZone) {
                    return dropZone as UI.DragDropZone;
                }
            }
            return null;
        }
    }

}
