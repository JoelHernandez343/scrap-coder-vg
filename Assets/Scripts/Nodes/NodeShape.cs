using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;


[Serializable]
public struct ShapePoint {
    public Vector2 position;
    public int spriteIndex;
}

public class NodeShape : MonoBehaviour, INodeExpander {

    [SerializeField] public SpriteShapeController spriteShapeController;
    [SerializeField] List<ShapePoint> shapePoints;

    [SerializeField] NodeTransform ownTransform;

    [SerializeField] Range widthPointsRange;
    [SerializeField] Range heightPointsRange;

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

    void INodeExpander.Expand(int dx, int dy, NodeTransform fromThistransform) {
        ownTransform.width += dx;
        ownTransform.height += dy;

        // Width
        for (var i = widthPointsRange.begin; i <= widthPointsRange.end; ++i) {
            var point = shapePoints[i];
            point.position.x += dx;
            shapePoints[i] = point;

            line.SetPosition(i, point.position / pixelsPerUnit);
        }

        // Height
        for (var i = heightPointsRange.begin; i <= heightPointsRange.end; ++i) {
            var point = shapePoints[i];
            point.position.y -= dy;
            shapePoints[i] = point;

            line.SetPosition(i, point.position / pixelsPerUnit);
        }
    }
}
