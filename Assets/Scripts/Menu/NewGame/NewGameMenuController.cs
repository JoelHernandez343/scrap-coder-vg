// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using ScrapCoder.Game;
using ScrapCoder.VisualNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ScrapCoder.UI
{
    public class NewGameMenuController : MonoBehaviour {

        // Editor variables
        [SerializeField] ButtonController returnButton;
        [SerializeField] ButtonController createButton;

        // Methods
        void Start() {
            returnButton.AddListener(() => SceneManager.LoadScene("Menu"));
            createButton.AddListener(() => ShowMessage());
        }

        void ShowMessage() {
            MessagesController.instance.AddMessage(
                message: "Hello world",
                isFinite: false,
                customHeight: 100
            );
        }
    }
}
