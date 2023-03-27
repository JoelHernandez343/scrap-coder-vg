// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using ScrapCoder.Game;
using ScrapCoder.VisualNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScrapCoder.UI {

    public class SavingDisplayButtonController : MonoBehaviour {

        // Editor variables
        [SerializeField] ExpandableText userIdText;
        [SerializeField] ExpandableText progressText;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= GetComponent<NodeTransform>();

        ButtonController _buttonController;
        ButtonController buttonController => _buttonController ??= GetComponent<ButtonController>();

        // Methods
        void Start() {
            buttonController.AddListener(() => GoToSelectLevel());
        }

        public void SetUserInformation(string userId, List<bool> levelProgress) {
            userIdText.ChangeText(userId);

            var totalLevels = levelProgress.Count;
            var completedLevels = levelProgress.FindIndex(level => level == false);
            completedLevels = completedLevels == -1 ? totalLevels : completedLevels;

            progressText.ChangeText($"{completedLevels}/{totalLevels}");
        }

        void GoToSelectLevel() {
            if (string.IsNullOrEmpty(userIdText.text)) throw new System.Exception("User information must be set first");

            LevelLoader.currentUserId = userIdText.text;
            SceneManager.LoadScene("LoadLevelScene");
        }

    }
}