// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Utils;
using ScrapCoder.UI;

namespace ScrapCoder.Game {
    public class LevelLoader : MonoBehaviour {

        public static string levelDataFileName = "levelData.json";

        [SerializeField] TextAsset levelsTemplate;

        [System.Serializable]
        public class Level {

            public string title;
            public string description;
            public string sceneName;
            public string spritePath;

        }

        private List<Level> _levels = null;
        public List<Level> levels => 
            _levels ??= Newtonsoft.Json.JsonConvert.DeserializeObject<List<Level>>(levelsTemplate.text);

        // Methods
        public void CreateLevelDataIfNotExists() {
            if (FileExists.PersistentFileExists(levelDataFileName)){
                return;
            }

            var lockedLevels = levels.ConvertAll(_ => false);
            StoreNewLevelCompletionData(lockedLevels);
        }

        public List<bool> GetLevelCompletionData() {
            CreateLevelDataIfNotExists();

            return SaveLoadJson<List<bool>>.LoadJsonFromPersistentData(
                subFilePath: levelDataFileName
            );
        }

        public void StoreNewLevelCompletionData(List<bool> newData) {
            SaveLoadJson<List<bool>>.SaveJsonToPersistentData(
                subFilePath: levelDataFileName,
                data: newData
            );
        }

    }
}