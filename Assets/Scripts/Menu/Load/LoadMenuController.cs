// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ScrapCoder.VisualNodes;
using ScrapCoder.Game;

namespace ScrapCoder.UI {
    public class LoadMenuController : MonoBehaviour {

        // editor variables
        [SerializeField] ButtonController returnButton;

        [SerializeField] ButtonController rightButton;
        [SerializeField] ButtonController leftbutton;

        [SerializeField] NodeTransform levelSelectorParent;

        [SerializeField] LevelLoader levelContainer;

        [SerializeField] LevelSelector levelSelectorPrefab;

        // State variables
        List<LevelSelector> selectors;
        int currentLevel = 0;

        // Methods
        void Start() {

            InitializeLevelSelectors();

            returnButton.AddListener(() => SceneManager.LoadScene("Menu"));

            rightButton.AddListener(() => ChangeLevel(forward: true));
            leftbutton.AddListener(() => ChangeLevel(forward: false));
        }

        void InitializeLevelSelectors() {
            var isUnlocked = true;
            var levelCompletionData = levelContainer.GetLevelCompletionData();
            var id = 0;

            selectors = levelContainer.levels.ConvertAll(
                level => {
                    var selector = LevelSelector.Create(
                        prefab: levelSelectorPrefab,
                        parent: levelSelectorParent.transform,
                        template: new LevelSelectorTemplate {
                            title = level.title,
                            description = level.description,
                            image = Resources.Load<Sprite>(level.spritePath),
                            isUnlocked = isUnlocked,
                            scene = level.sceneName
                        }
                    );

                    if (isUnlocked && levelCompletionData[id] == false) {
                        isUnlocked = false;
                    }

                    id += 1;

                    return selector;
                }
            );

            for (var i = 0; i < selectors.Count; ++i) {
                selectors[i].ownTransform.SetPosition(
                    x: (1920 / 2) * (i + 1) + (1920 / 2) * i,
                    y: -700 / 2
                );
            }
        }

        void ChangeLevel(bool forward) {
            if (forward) {
                currentLevel += currentLevel == selectors.Count - 1 ? 0 : 1;
            } else {
                currentLevel -= currentLevel == 0 ? 0 : 1;
            }

            PositionOnLevel(level: currentLevel);
        }

        void PositionOnLevel(int level) {
            levelSelectorParent.SetPosition(
                x: -((1920 / 2) * (level + 1) + (1920 / 2) * level),
                smooth: true
            );
        }

    }
}