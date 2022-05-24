using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ScrapCoder.GameInput;

public class NextScene : MonoBehaviour
{
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
                    SceneManager.LoadScene("Level 1-2");
                    break;
                case "Level 1-2":
                    SceneManager.LoadScene("Level 1-3");
                    break;
                case "Level 1-3":
                    SceneManager.LoadScene("Level 1-4");
                    break;
                case "Level 1-4":
                    SceneManager.LoadScene("Level 1-5");
                    break;
                default:
                    SceneManager.LoadScene("Menu");
                    break;

            }
        }
    }
}
