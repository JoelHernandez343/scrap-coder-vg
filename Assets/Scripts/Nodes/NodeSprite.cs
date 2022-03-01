// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScrapCoder.VisualNodes {

    public class NodeSprite : MonoBehaviour {
        [SerializeField] public NodeTransform ownTransform;
        [SerializeField] new SpriteRenderer renderer;
        [SerializeField] bool hideable;

        public void Hide() => ToggleRender(false);
        public void Show() => ToggleRender(true);

        public void ToggleRender(bool render) {
            if (hideable) {
                renderer.enabled = render;
            }
        }
    }

}