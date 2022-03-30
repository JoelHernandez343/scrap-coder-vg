// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeZone : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] new PolygonCollider2D collider;
        [SerializeField] NodeTransform ownTransform;

        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        // State variables
        [SerializeField] ZoneColor color;

        List<NodeZone> zones = new List<NodeZone>();

        bool isActive = true;

        // Lazy and other variables
        public ZoneColor zoneColor {
            private set => color = value;
            get => color;
        }

        List<NodeRange> _ranges;
        List<NodeRange> ranges
            => _ranges ??= new List<NodeRange> { widthPointsRange, heightPointsRange };

        List<Vector2> _colliderPoints;
        List<Vector2> colliderPoints
            => _colliderPoints ??= new List<Vector2>(collider.GetPath(0));

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

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, NodeArray _) {
            int[] delta = { dx, dy };

            for (int axis = 0; axis < ranges.Count; ++axis) {
                var range = ranges[axis];
                var isExpandable = range.isExpandable;

                var sign = axis == 0 ? 1 : -1;

                if (!isExpandable) continue;

                for (var i = range.begin; i <= range.end; ++i) {
                    var point = colliderPoints[i];
                    point[axis] += (sign) * delta[axis];
                    colliderPoints[i] = point;
                }
            }

            collider.SetPath(0, colliderPoints);

            return (dx, dy);
        }
    }

}
