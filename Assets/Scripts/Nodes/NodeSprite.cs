// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScrapCoder.VisualNodes {

    public class NodeSprite : MonoBehaviour {

        // Editor variables
        [SerializeField] List<Sprite> availableSprites;

        [SerializeField] bool hideable;

        int selectedSprite;

        // Lazy state variables
        Utils.Random _rand;
        Utils.Random rand {
            get => _rand ??= new Utils.Random();
            set => _rand = value;
        }


        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        SpriteRenderer _spriteRenderer;
        SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();

        // Methods
        void Awake() {
            ChangeSprite();
        }

        public void SetVisible(bool visible) {
            if (hideable) {
                spriteRenderer.enabled = visible;
            }
        }

        public void SetSeed(int seed) {
            rand = new Utils.Random(seed);
            ChangeSprite();
        }

        void ChangeSprite() {
            if (availableSprites.Count != 0) {
                selectedSprite = rand.NextIntRange(0, availableSprites.Count - 1);
                spriteRenderer.sprite = availableSprites[selectedSprite];
            }
        }
    }

}