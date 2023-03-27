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
            inputText.AddListener(() => CreateUser());
            acceptMessageButton.AddListener(() => StartAgain());
        }

        void CreateUser() {
            inputUserId = inputText.Value;

            if (string.IsNullOrEmpty(inputUserId)) {
                MessagesController.instance.AddMessage(
                    message: "Ingresa un valor no vacío",
                    hideInNewMessage: true,
                    status: MessageStatus.Error,
                    seconds: 10
                );
                return;
            };

            if (!Regex.IsMatch(inputUserId, "^[^\\s]+.*[^\\s]+$")) {
                MessagesController.instance.AddMessage(
                    message: "El usuario no puede tener espacios ni al principio, ni al final",
                    hideInNewMessage: true,
                    status: MessageStatus.Error,
                    seconds: 10
                );
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
                    hideInNewMessage: true,
                    status: MessageStatus.Warning,
                    type: MessageType.AcceptAndCancel
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
