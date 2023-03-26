// Edited by
// Joel Harim Hernández Javier @ Febreary 2023
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ScrapCoder.GameInput;
using ScrapCoder.Game;
using System.Linq;

public class NextScene : MonoBehaviour {
    // Editor variables
    [SerializeField] LevelLoader levelContainer;

    // Update is called once per frame
    void Update() {
        if (InputController.instance.GetButtonDown("Reset")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag != "player") return;

        var levels = levelContainer.levels;
        var currentSceneName = SceneManager.GetActiveScene().name;

        var id = levels.FindIndex(level => level.sceneName == currentSceneName);
        var nextSceneName = levels.ElementAtOrDefault(id + 1)?.sceneName ?? "Menu";

        levelContainer.StoreCurrentLevelProgress(id);

        SceneManager.LoadScene(nextSceneName);
    }
}
