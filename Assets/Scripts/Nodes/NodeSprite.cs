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

        // Lazy state variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        SpriteRenderer _spriteRenderer;
        SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();

        int? _selectedSprite;
        int selectedSprite {
            set { _selectedSprite = value; }
            get {
                _selectedSprite ??= Utils.Random.NextRange(0, availableSprites.Count - 1);

                return (int)_selectedSprite;
            }
        }

        // Methods
        void Awake() {
            if (availableSprites.Count != 0) {
                spriteRenderer.sprite = availableSprites[selectedSprite];
            }
        }

        public void SetVisible(bool visible) {
            if (hideable) {
                spriteRenderer.enabled = visible;
            }
        }
    }

}