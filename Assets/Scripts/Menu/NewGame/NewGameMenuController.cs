// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using ScrapCoder.Game;
using ScrapCoder.VisualNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ScrapCoder.UI
{
    public class NewGameMenuController : MonoBehaviour {

        // Editor variables
        [SerializeField] ButtonController returnButton;
        [SerializeField] ButtonController createButton;
        [SerializeField] ButtonController acceptMessageButton;

        [SerializeField] InputText inputText;
        [SerializeField] LevelLoader levelContainer;

        string inputUserId;

        // Methods
        void Start() {
            returnButton.AddListener(() => SceneManager.LoadScene("Menu"));
            createButton.AddListener(() => CreateUser());
            acceptMessageButton.AddListener(() => StartAgain());
        }

        void CreateUser() {
            inputUserId = inputText.Value;

            if (string.IsNullOrEmpty(inputUserId)) {
                // Here sould be a message
                return;
            };

            if (!Regex.IsMatch(inputUserId, "^[^\\s]+.*[^\\s]+$")) {
                // Here sould be a message
                return;
            }

            var users = LevelLoader.GetAllLevelProgressData();

            if (!users.ContainsKey(inputUserId)) {
                LevelLoader.AddUser(inputUserId, levelContainer.levels.Count);
                LevelLoader.currentUserId = inputUserId;
                SceneManager.LoadScene("Cinematic");
            } else {
                MessagesController.instance.AddMessage(
                    message: "Este usuario ya existe, ¿quieres empezar de nuevo la aventura?",
                    isFinite: false,
                    customHeight: 100,
                    hideInNewMessage: true
                );
            }
        }

        void StartAgain() {
            LevelLoader.currentUserId = inputUserId;
            // LevelLoader.ResetCurrentLevelProgress();
            SceneManager.LoadScene("Cinematic");
        }
    }
}
