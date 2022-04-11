// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {

    public class DragDropZone : MonoBehaviour, INodeDropHandler {

        // Editor variables
        [SerializeField] public string category;

        // State variables
        List<NodeZone> zones = new List<NodeZone>();

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        bool INodeDropHandler.IsActive => true;
        Transform INodeDropHandler.Transform => transform;
        NodeTransform INodeDropHandler.OwnTransform => ownTransform;

        // Methods
        bool INodeDropHandler.OnDrop(NodeZone _) => false;

        public void OnTriggerEnter2D(Collider2D collider) {
            var zone = (collider.GetComponent<NodeZone>() as NodeZone);

            if (zone == null) return;
            if (zone.controller.isDragging == false) return;
            if (zone.controller.validZones.IndexOf(zone) == -1) return;

            zones.Add(zone);

            if (zones.Count == zone.controller.validZones.Count) {
                SetState("over");
            }
        }

        public void OnTriggerExit2D(Collider2D collider) {
            var zone = (collider.GetComponent<NodeZone>() as NodeZone);

            if (zone == null) return;
            if (zone.controller.isDragging == false) return;

            if (zones.Remove(zone)) {
                SetState("normal");
            }
        }

        public void SetState(string state) {
            if (state == "over") {
                // Here switch to over state
            } else if (state == "normal" && zones.Count != 0) {
                zones.Clear();
                // Here switch to normal state
            }
        }
    }

}