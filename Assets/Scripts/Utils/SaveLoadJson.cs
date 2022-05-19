// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using SimpleFileBrowser;

namespace ScrapCoder.Utils {
    public static class SaveLoadJson<T> {

        public static void SaveJsonToFile(T data, string filePath) {
            var folderName = FileBrowserHelpers.GetDirectoryName(filePath);
            var fileName = FileBrowserHelpers.GetFilename(filePath);
            var jsonData = JsonConvert.SerializeObject(data);

            try {
                FileBrowserHelpers.WriteTextToFile(filePath, jsonData);
            } catch (System.UnauthorizedAccessException e) {
                Debug.LogError(e);
            }
        }

        public static void SaveJsonToPersistentData(T data, string subFilePath) {
            SaveJsonToFile(data: data, filePath: Path.Combine(Application.persistentDataPath, subFilePath));
        }

        public static T LoadJsonFromFile(string filePath) {

            var jsonData = FileBrowserHelpers.ReadTextFromFile(filePath);
            T data = JsonConvert.DeserializeObject<T>(jsonData);

            return data;
        }

    }
}