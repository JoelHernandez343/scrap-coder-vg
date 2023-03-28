// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using ScrapCoder.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScrapCoder.UI {

    public class SelectGameMenuController : MonoBehaviour {

        // Editor variables
        [SerializeField] ButtonController returnButton;
        [SerializeField] ButtonController rightButton;
        [SerializeField] ButtonController leftbutton;

        [SerializeField] List<SavingDisplayButtonController> saveButtons;

        // State variables
        IOrderedEnumerable<KeyValuePair<string, List<bool>>> sortedUsers;
        int sortedUsersCount;

        int currentPage = 0;

        // Lazy variables
        int pageSize => saveButtons.Count;

        // Methods
        void Start() {
            saveButtons.ForEach(button => ResetSavingButtonPosition(button));

            returnButton.AddListener(() => SceneManager.LoadScene("Menu"));

            rightButton.AddListener(() => ChangPage(forward: true));
            leftbutton.AddListener(() => ChangPage(forward: false));

            FilterUsersBy("");
        }

        public void FilterUsersBy(string partialUserId) { 
            string regexPattern = $"^{Regex.Escape(partialUserId)}.*$";

            sortedUsers = LevelLoader.GetAllLevelProgressData()
                .Where(pair => Regex.IsMatch(pair.Key, regexPattern, RegexOptions.IgnoreCase))
                .OrderBy(pair => pair.Key);
            sortedUsersCount = sortedUsers.Count();

            ShowPage(0);
        }

        void ResetSavingButtonPosition(SavingDisplayButtonController saveButton) {
            saveButton.ownTransform.SetPosition(x: 3000);
        }

        void ChangPage(bool forward) {
            if (forward && (currentPage + 1) * pageSize <= sortedUsersCount) { 
                currentPage += 1;
                ShowPage(currentPage);
            } else if (!forward && currentPage - 1 >= 0) { 
                currentPage -= 1;
                ShowPage(currentPage);
            }
        }

        void ShowPage(int pageNumber) {
            var users = sortedUsers.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            DisplaySaves(users);
        }

        void DisplaySaves(List<KeyValuePair<string, List<bool>>> users){
            for (var i = 0; i < saveButtons.Count; i++) { 
                if (i < users.Count) {
                    saveButtons[i].ownTransform.SetPosition(x: -saveButtons[i].ownTransform.width);
                    saveButtons[i].SetUserInformation(userId: users[i].Key, levelProgress: users[i].Value);
                } else {
                    ResetSavingButtonPosition(saveButtons[i]);
                }
            }
        }
    }
}