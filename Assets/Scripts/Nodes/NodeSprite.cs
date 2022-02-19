// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScrapCoder.VisualNodes {

    public class NodeSprite : MonoBehaviour {
        [SerializeField] NodeTransform ownTransform;
        [SerializeField] new SpriteRenderer renderer;
        [SerializeField] bool hideable;

        public void hide() => toggleRender(false);
        public void show() => toggleRender(true);

        public void toggleRender(bool render) {
            if (hideable) {
                renderer.enabled = render;
            }
        }
    }

}