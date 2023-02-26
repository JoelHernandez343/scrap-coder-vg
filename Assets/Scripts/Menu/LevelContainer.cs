// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.Utils;

namespace ScrapCoder.Game {
    public class LevelContainer : MonoBehaviour {

        public static string levelDataFileName = "levelData.json";

        [System.Serializable]
        public class Level {

            public Sprite image;
            public string title;
            public string description;
            public string sceneName;

        }

        [SerializeField] public List<Level> levels;

        // Methods
        public void CreateLevelDataIfNotExists() {
            if (FileExists.PersistentFileExists(levelDataFileName)){
                return;
            }

            var storedLevels = levels.ConvertAll(
                level => new StoredLevelTemplate() {
                    isUnlocked = false,
                    sceneName = level.sceneName,
                }
            );

            SaveLoadJson<List<StoredLevelTemplate>>.SaveJsonToPersistentData(
                data: storedLevels,
                subFilePath: levelDataFileName
            ); 
        }

        public List<StoredLevelTemplate> GetStoredLevelData() {
            CreateLevelDataIfNotExists();

            return SaveLoadJson<List<StoredLevelTemplate>>.LoadJsonFromPersistentData(
                subFilePath: levelDataFileName
            );
        }

        public void StoreNewLevelData(List<StoredLevelTemplate> newData) {
            SaveLoadJson<List<StoredLevelTemplate>>.SaveJsonToPersistentData(
                subFilePath: levelDataFileName,
                data: newData
            );
        }

    }
}