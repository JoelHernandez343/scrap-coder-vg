/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeContainer {

    NodeController3 controller;

    float horizontalOffset {
        get => asidePart.width;
    }

    NodePart asidePart;
    NodePart bottomPart;

    public NodeList nodes;

    public (float x, float y) position { private set; get; }

    [SerializeField] public float height;
    [SerializeField] public float width;

    public (float x, float y) endPosition => (position.x + width, position.y - height);

    public NodeContainer(NodeController3 controller, NodePart asidePart, NodePart bottomPart) {
        this.controller = controller;
        this.asidePart = asidePart;
        this.bottomPart = bottomPart;

        nodes = new NodeList(controller);
        nodes.container = this;

        height = 15 + bottomPart.height;
        width = bottomPart.width;
        nodes.SetPosition((position.x + asidePart.width, position.y), init: true);
    }

    public void SetPartsPosition(NodeController3 node = null) {

        var internalHeight = nodes.Count == 0 ? 15 : nodes.height - 1;

        nodes.SetPosition((position.x + asidePart.width, position.y));
        GrowAsidePart(internalHeight);
        bottomPart.SetPosition((position.x, position.y - internalHeight));

        SetDimensions(nodes.Count == 0);

        controller.SetPartsPosition(node);
    }

    void GrowAsidePart(float height) {
        asidePart.SetPosition(position);
        asidePart.Grow(height);
    }

    void SetDimensions(bool initial) {
        if (initial) {
            height = 15 + bottomPart.height;
            width = bottomPart.width;
        } else {
            height = nodes.height + bottomPart.height - 1;
            width = nodes.width > bottomPart.width ? nodes.width : bottomPart.width;
        }
    }

    public (float x, float y) SetPosition((float x, float y) position) {
        if (this.position == position) {
            return endPosition;
        }

        this.position = position;

        SetPartsPosition();

        return endPosition;
    }

}
