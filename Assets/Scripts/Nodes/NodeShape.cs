// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace ScrapCoder.VisualNodes {

    [System.Serializable]
    public struct ShapePoint {
        public Vector2 position;
        public int spriteIndex;
    }

    public class NodeShape : MonoBehaviour, INodeExpander {

        [SerializeField] public SpriteShapeController spriteShapeController;
        [SerializeField] List<ShapePoint> shapePoints;

        [SerializeField] NodeTransform ownTransform;

        [SerializeField] NodeRange widthPointsRange;
        [SerializeField] NodeRange heightPointsRange;

        public Spline line => spriteShapeController?.spline;

        int pixelsPerUnit;

        void Awake() {
            pixelsPerUnit = NodeTransform.PixelsPerUnit;

            SetDefaultShape();
        }

        void SetDefaultShape() {
            line.Clear();

            for (var i = 0; i < shapePoints.Count; ++i) {
                var point = shapePoints[i];

                line.InsertPointAt(i, point.position / pixelsPerUnit);
                line.SetSpriteIndex(i, point.spriteIndex);
                line.SetTangentMode(i, ShapeTangentMode.Linear);
            }

            spriteShapeController.RefreshSpriteShape();
        }

        void INodeExpander.Expand(int dx, int dy) {
            ownTransform.width += dx;
            ownTransform.height += dy;

            // Width
            if (widthPointsRange.begin > -1) {
                for (var i = widthPointsRange.begin; i <= widthPointsRange.end; ++i) {
                    var point = shapePoints[i];
                    point.position.x += dx;
                    shapePoints[i] = point;

                    line.SetPosition(i, point.position / pixelsPerUnit);
                }
            }

            // Height
            if (heightPointsRange.begin > -1) {
                for (var i = heightPointsRange.begin; i <= heightPointsRange.end; ++i) {
                    var point = shapePoints[i];
                    point.position.y -= dy;
                    shapePoints[i] = point;

                    line.SetPosition(i, point.position / pixelsPerUnit);
                }
            }

        }
    }
}