// Joel Harim Hernández Javier @ 2023
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Utils;
using ScrapCoder.UI;
using UnityEngine.UIElements;
using System.Linq;

namespace ScrapCoder.Game {
    public class LevelLoader : MonoBehaviour {

        // Static variables
        public static string levelDataFileName = "levelData.json";

        static Dictionary<string, List<bool>> progressData = null;
        public static string currentUserId = null;

        // Internal types
        public class Level
        {
            public string title;
            public string description;
            public string sceneName;
            public string spritePath;
        }

        // Editor variables
        [SerializeField] TextAsset levelsTemplate;

        // Lazy variables
        private List<Level> _levels = null;
        public List<Level> levels => 
            _levels ??= Newtonsoft.Json.JsonConvert.DeserializeObject<List<Level>>(levelsTemplate.text);

        // Methods
        public static Dictionary<string, List<bool>> GetAllLevelProgressData() { 
            if (progressData != null) return progressData;

            if (FileExists.PersistentFileExists(levelDataFileName)) { 
                progressData = SaveLoadJson<Dictionary<string, List<bool>>>.LoadJsonFromPersistentData(
                    subFilePath: levelDataFileName
                );

                if (progressData != null ) { Debug.Log("progressData loaded"); }
                else { Debug.Log("progress data created"); }
            } 

            if (progressData == null) { 
                progressData = new Dictionary<string, List<bool>>();
                SaveAllLevelProgressData(progressData);
            }

            return progressData;
        }

        static void SaveAllLevelProgressData(Dictionary<string, List<bool>> progressData) {
            SaveLoadJson<Dictionary<string, List<bool>>>.SaveJsonToPersistentData(
                subFilePath: levelDataFileName,
                data: progressData
            );
        }

        public static void StoreCurrentLevelProgress(int levelId, bool isCompleted = true){
            if (currentUserId == null) throw new System.Exception("User id must be set first");

            var progressData = GetAllLevelProgressData();

            progressData[currentUserId][levelId] = isCompleted;

            SaveAllLevelProgressData(progressData);
        }

        public static void ResetCurrentLevelProgress() { 
            if (currentUserId == null) throw new System.Exception("User id must be set first");

            var progressData = GetAllLevelProgressData();

            progressData[currentUserId] = progressData[currentUserId].ConvertAll(_ => false);

            SaveAllLevelProgressData(progressData);
        }


        public static List<bool> GetCurrentLevelProgress() {
            if (currentUserId == null) throw new System.Exception("User id must be set first");

            var progressData = GetAllLevelProgressData();

            return progressData[currentUserId];
        }

        public static void AddUser(string userId, int levelCount) {
            progressData[userId] = Enumerable.Range(0, levelCount).Select(_ => false).ToList();

            SaveAllLevelProgressData(progressData);
        }
    }
}