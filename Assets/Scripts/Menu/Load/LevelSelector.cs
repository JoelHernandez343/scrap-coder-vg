// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class LevelSelector : MonoBehaviour {

        // Editor variables
        [SerializeField] ExpandableText titleText;
        [SerializeField] ExpandableText descriptionText;

        [SerializeField] NodeTransform descriptionContainer;

        [SerializeField] ButtonController playButton;

        [SerializeField] SpriteRenderer imageSprite;
        [SerializeField] SpriteRenderer fadeSprite;

        // State variables
        bool isUnlocked;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        // Methods
        public void Initialize(LevelSelectorTemplate template) {

            var dxTitle = titleText.ChangeText(newText: template.title);
            var dxDescription = descriptionText.ChangeText(newText: template.description);

            descriptionContainer.Expand(
                dx: dxTitle > dxDescription ? dxTitle : dxDescription
            );
            descriptionContainer.SetPosition(x: -descriptionContainer.width);

            titleText.ownTransform.SetPosition(
                x: descriptionContainer.width - titleText.ownTransform.width
            );
            descriptionText.ownTransform.SetPosition(
                x: descriptionContainer.width - descriptionText.ownTransform.width
            );


            playButton.text = template.isUnlocked ? "Jugar" : "Bloqueado";
            playButton.ExpandByText();
            playButton.ownTransform.SetPosition(x: -playButton.ownTransform.width);

            if (template.image != null) {
                imageSprite.sprite = template.image;
            }

            fadeSprite.gameObject.SetActive(!template.isUnlocked);

            isUnlocked = template.isUnlocked;

            playButton.AddListener(() => {
                if (isUnlocked) {
                    SceneManager.LoadScene(sceneName: template.scene);
                }
            });
        }

        static public LevelSelector Create(LevelSelector prefab, Transform parent, LevelSelectorTemplate template) {
            var newLevelSelector = Instantiate(original: prefab, parent: parent);

            newLevelSelector.ownTransform.depth = 0;
            newLevelSelector.ownTransform.SetScale(x: 1, y: 1, z: 1);
            newLevelSelector.Initialize(template: template);

            return newLevelSelector;
        }

    }
}