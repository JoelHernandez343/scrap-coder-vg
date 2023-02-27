// Edited by
// Joel Harim Hernández Javier @ Febreary 2023
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ScrapCoder.GameInput;
using ScrapCoder.Game;

public class NextScene : MonoBehaviour
{
    // Editor variables
    [SerializeField] LevelLoader levelContainer;

    // Update is called once per frame
    void Update()
    {
        if (InputController.instance.GetButtonDown("Reset"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var levelCompletionData = levelContainer.GetLevelCompletionData();
            var levels = levelContainer.levels;

            var currentSceneName = SceneManager.GetActiveScene().name;
            var nextSceneName = "";

            // Search for current level, update locked status, and get the next scene name (if is the last, return to Menu)
            for (int id = 0; id < levels.Count; id++) {
                if (levels[id].sceneName == currentSceneName) {
                    levelCompletionData[id] = true;
                    nextSceneName = id < levelCompletionData.Count - 1
                        ? levels[id + 1].sceneName
                        : "Menu";
                    break;
                }
            }

            // Store the updated list
            levelContainer.StoreNewLevelCompletionData(newData: levelCompletionData);

            // Load the next scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
