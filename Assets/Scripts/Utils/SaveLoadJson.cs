// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace ScrapCoder.Utils {
    public static class SaveLoadJson<T> {

        public static bool SaveJsonToFile(T data, string filePath) {
            var jsonData = JsonConvert.SerializeObject(data);

            try {
                File.WriteAllText(filePath, jsonData);
                return true;
            } catch (System.UnauthorizedAccessException e) {
                Debug.LogError(e);
                return false;
            }
        }

        public static bool SaveJsonToPersistentData(T data, string subFilePath) {
            return SaveJsonToFile(data: data, filePath: Path.Combine(Application.persistentDataPath, subFilePath));
        }

        public static T LoadJsonFromFile(string filePath) {
            var jsonData = File.ReadAllText(filePath);
            T data = JsonConvert.DeserializeObject<T>(jsonData);

            return data;
        }

        public static T LoadJsonFromPersistentData(string subFilePath) {
            var jsonData = File.ReadAllText(Path.Combine(Application.persistentDataPath, subFilePath));
            T data = JsonConvert.DeserializeObject<T>(jsonData);

            return data;
        }

    }
}