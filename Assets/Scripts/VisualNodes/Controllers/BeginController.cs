/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginController : NodeController3 {

    NodePart part;

    public override void Init() {
        part = GetComponentInChildren<NodePart>();
        part.controller = this;

        SetDimensions();

        siblings.SetPosition((0, -part.height), init: true);
    }

    public override void SetDimensions() {
        (height, width) = (part.height, part.width);
    }
}