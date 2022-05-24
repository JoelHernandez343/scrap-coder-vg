using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ScrapCoder.GameInput;
using ScrapCoder.Game;

public class NextScene : MonoBehaviour
{
    // Editor variables
    [SerializeField] LevelContainer levelContainer;

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
            switch (SceneManager.GetActiveScene().name)
            {
                case "Level 1-1":
                    SetLevelUnlocked(id: 0);
                    SceneManager.LoadScene("Level 1-2");
                    break;
                case "Level 1-2":
                    SetLevelUnlocked(id: 1);
                    SceneManager.LoadScene("Level 1-3");
                    break;
                case "Level 1-3":
                    SetLevelUnlocked(id: 2);
                    SceneManager.LoadScene("Level 1-4");
                    break;
                case "Level 1-4":
                    SetLevelUnlocked(id: 3);
                    SceneManager.LoadScene("Level 1-5");
                    break;
                default:
                    SetLevelUnlocked(id: 4);
                    SceneManager.LoadScene("Menu");
                    break;

            }
        }
    }

    void SetLevelUnlocked(int id){

        var storedLevelData = levelContainer.GetStoredLevelData();
        storedLevelData[id].isUnlocked = true;
        levelContainer.StoreNewLevelData(newData: storedLevelData);
    }
}
