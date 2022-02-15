// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScrapCoder.VisualNodes {

    public class NodeSprite : MonoBehaviour {
        [SerializeField] NodeTransform ownTransform;
        [SerializeField] new SpriteRenderer renderer;

        public void hide() => toggleRender(false);
        public void show() => toggleRender(true);

        void toggleRender(bool render) => renderer.enabled = render;
    }

}