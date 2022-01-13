/*
 * Joel Hernández
 * Github: https://github.com/JoelHernandez343
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePart : MonoBehaviour {

    public RectTransform rectTransform;

    public (float x, float y) position {
        get {
            var x = rectTransform.anchoredPosition.x;
            var y = rectTransform.anchoredPosition.y;

            return (x, y);
        }
        private set {
            if (this.position == value) {
                return;
            }

            var position = new Vector2(value.x, value.y);
            rectTransform.anchoredPosition = position;
        }
    }

    [SerializeField] public float height;
    [SerializeField] public float width;

    public (float x, float y) endPosition {
        get {
            var (x, y) = position;
            return (x + width, y - height);
        }
    }

    public NodeController3 controller { set; get; }

    public void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Grow(float? height = null, float? width = null) {
        if (height is float h) {
            this.height = h < 0 ? -h : h;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, this.height);
        }

        if (width is float w) {

        }
    }

    public (float x, float y) SetPosition((float x, float y) position) {
        if (this.position != position) {
            this.position = position;
        }

        return endPosition;
    }
}
