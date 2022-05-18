// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {

    public class SpawnerSelectionContainerExpander : MonoBehaviour, INodeExpander {

        // Editor variables
        [SerializeField] NodeTransform sideBorders;
        [SerializeField] ScrollBarController scrollBar;

        [SerializeField] NodeTransform content;
        [SerializeField] NodeTransform parentContent;


        // Lazy variables
        SpawnerSelectionContainer _selectionContainer;
        SpawnerSelectionContainer selectionContainer => _selectionContainer ??= GetComponent<SpawnerSelectionContainer>() as SpawnerSelectionContainer;

        NodeTransform ownTransform => selectionContainer.ownTransform;

        // Methods
        public (int? dx, int? dy) Expand(int? dx = null, int? dy = null, bool smooth = false, INodeExpanded expanded = null) {
            sideBorders.Expand(dx: dx, dy: dy, smooth: smooth);

            content.initHeight += dy ?? 0;

            // For future
            // if (content.initHeight > selectionContainer.lastSpawnerY) {
            //     content.Expand(dy: content.initHeight - content.height);
            // }

            var visor = (ownTransform.height + dy) - (-parentContent.y) ?? scrollBar.visor;

            scrollBar.visor = visor;
            scrollBar.ownTransform.Expand(dy: dy);
            scrollBar.RefreshSlider();

            return (dx, dy);
        }
    }
}