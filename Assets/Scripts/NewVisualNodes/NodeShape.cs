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

    [SerializeField] public SpriteShapeController shapeController;
    [SerializeField] List<ShapePoint> shapePoints;
    [SerializeField] NodeTransform nodeTransform;

    [SerializeField] Range widthPointsRange;
    [SerializeField] Range heightPointsRange;

    public Spline line => shapeController?.spline;

    void Awake() {
        SetDefaultShape();
    }

    void SetDefaultShape() {
        line.Clear();

        for (var i = 0; i < shapePoints.Count; ++i) {
            var point = shapePoints[i];

            line.InsertPointAt(i, point.position);
            line.SetSpriteIndex(i, point.spriteIndex);
            line.SetTangentMode(i, ShapeTangentMode.Linear);
        }

        shapeController.RefreshSpriteShape();
    }

    void INodeExpander.Expand(int dx, int dy) {
        nodeTransform.width += dx;
        nodeTransform.height += dy;

        // Width
        for (var i = widthPointsRange.begin; i <= widthPointsRange.end; ++i) {
            var vector = line.GetPosition(i);
            vector.x += dx;
            line.SetPosition(i, vector);
        }

        // Height
        for (var i = heightPointsRange.begin; i <= heightPointsRange.end; ++i) {
            var vector = line.GetPosition(i);
            vector.y -= dy;
            line.SetPosition(i, vector);
        }
    }
}
