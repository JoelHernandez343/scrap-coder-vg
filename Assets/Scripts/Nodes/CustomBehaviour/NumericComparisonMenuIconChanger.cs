// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {

    public class NumericComparisonMenuIconChanger : MonoBehaviour {

        // Editor variables
        [SerializeField] NodeSprite comparisonIcon;

        // Lazy variables
        DropMenuController _dropMenu;
        DropMenuController dropMenu => _dropMenu ??= (GetComponent<DropMenuController>() as DropMenuController);

        // Methods
        void Awake() {
            dropMenu.AddListener(() => ChangeIcon(operation: dropMenu.Value));
        }

        public void ChangeIcon(string operation) {
            if (operation == "0") {
                comparisonIcon.SetState("isEqual");
            } else if (operation == "1") {
                comparisonIcon.SetState("isNotEqual");
            } else if (operation == "2") {
                comparisonIcon.SetState("isLessThan");
            } else if (operation == "3") {
                comparisonIcon.SetState("isLessOrEqual");
            } else if (operation == "4") {
                comparisonIcon.SetState("isGreaterThan");
            } else if (operation == "5") {
                comparisonIcon.SetState("isGreaterOrEqual");
            }
        }

    }

}