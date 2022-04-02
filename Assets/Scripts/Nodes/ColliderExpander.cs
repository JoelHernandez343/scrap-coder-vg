// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class ColliderExpander : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] PolygonCollider2D polygonCollider;

        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        // Lazy and other variables
        List<NodeRange> _ranges;
        List<NodeRange> ranges
            => _ranges ??= new List<NodeRange> { widthPointsRange, heightPointsRange };

        List<Vector2> _colliderPoints;
        List<Vector2> colliderPoints
            => _colliderPoints ??= new List<Vector2>(polygonCollider.GetPath(0));

        (int dx, int dy) INodeExpander.Expand(int dx, int dy, bool smooth, NodeArray fromThisArray) {
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

            polygonCollider.SetPath(0, colliderPoints);

            return (dx, dy);
        }
    }
}