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

    // Lazy variables
    string currentSceneName => SceneManager.GetActiveScene().name;

    // Update is called once per frame
    void Update() {
        if (InputController.instance.GetButtonDown("Reset")) {
            SceneManager.LoadScene(currentSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag != "Player") return;

        var levels = levelContainer.levels;

        var id = levels.FindIndex(level => level.sceneName == currentSceneName);
        var nextSceneName = levels.ElementAtOrDefault(id + 1)?.sceneName ?? "Menu";

        LevelLoader.StoreCurrentLevelProgress(id);
        SceneManager.LoadScene(nextSceneName);
    }
}
