// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScrapCoder.VisualNodes {

    public class NodeSprite : MonoBehaviour {

        // Editor variables
        [SerializeField] public NodeTransform ownTransform;
        [SerializeField] new SpriteRenderer renderer;

        [SerializeField] List<Sprite> availableSprites;

        // State Variables
        [SerializeField] bool hideable;

        // Lazy state variables
        int? _selectedSprite;
        int selectedSprite {
            set { _selectedSprite = value; }
            get {
                _selectedSprite ??= random.NextRange(0, availableSprites.Count - 1);

                return (int)_selectedSprite;
            }
        }

        // Lazy and other variables
        Utils.Random _random;
        Utils.Random random {
            get {
                _random ??= new Utils.Random();
                return _random;
            }
        }

        // Methods
        void Awake() {
            renderer.sprite = availableSprites[selectedSprite];
        }

        void Hide() => ToggleRender(false);
        void Show() => ToggleRender(true);

        public void ToggleRender(bool render) {
            if (hideable) {
                renderer.enabled = render;
            }
        }
    }

}