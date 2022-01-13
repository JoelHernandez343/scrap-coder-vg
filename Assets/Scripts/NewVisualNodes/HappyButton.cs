using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappyButton : MonoBehaviour {

    [SerializeField] NodeTransform piece;
    [SerializeField] bool grow = true;

    // Start is called before the first frame update
    void Awake() {
        GetComponent<Button>().onClick.AddListener(Grow);
    }

    public void Grow() {
        var dx = grow ? 1 : -1;
        var dy = grow ? 1 : -1;

        piece.Expand(dx, dy);
    }
}
