// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class SpawnerSelectionContainerLockButton : MonoBehaviour {
        // State variables
        public bool isLocked = true;

        // Lazy variables
        ButtonController _button;
        ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        // Methods
        void Start() {
            ChangeSprite();

            button.AddListener(() => {
                isLocked = !isLocked;
                ChangeSprite();
            });
        }

        void ChangeSprite() {
            button.spriteState.ChangeSelectedSprite(isLocked ? 0 : 1);
        }
    }
}