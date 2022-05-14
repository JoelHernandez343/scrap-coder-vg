using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
                SceneManager.LoadScene("Menu");
                break;
        }
    }
}
