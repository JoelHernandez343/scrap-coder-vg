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

        [SerializeField] bool selectRandomSprite = true;

        [SerializeField]
        List<string> states = new List<string> {
            "normal",
            "over",
            "pressed",
            "disabled"
        };

        [SerializeField] int spriteRange;

        // State variables
        int state;
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
            SelectRandomSprite();
        }

        public void SetVisible(bool visible) {
            if (hideable) {
                spriteRenderer.enabled = visible;
            }
        }

        public void SetSeed(int seed) {
            rand = new Utils.Random(seed);
            SelectRandomSprite();
        }

        void SelectRandomSprite() {
            if (!selectRandomSprite) return;

            if (spriteRange > 0) {
                selectedSprite = rand.NextIntRange(0, spriteRange);
                ChangeSprite();
            }
        }

        public void ChangeSelectedSprite(int newSprite) {
            selectedSprite = newSprite;
            ChangeSprite();
        }

        void ChangeSprite() {
            spriteRenderer.sprite = availableSprites[selectedSprite + (this.state * spriteRange)];

            if (ownTransform != null) {
                ownTransform.width = (int)System.Math.Round(spriteRenderer.sprite.rect.width);
                ownTransform.height = (int)System.Math.Round(spriteRenderer.sprite.rect.height);
            }
        }

        public void SetState(string state) {
            var stateIndex = states.IndexOf(state);

            if (stateIndex != -1) {
                this.state = stateIndex;
                ChangeSprite();
            }
        }
    }

}