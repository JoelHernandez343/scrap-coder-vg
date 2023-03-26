// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;

namespace ScrapCoder.Utils {
    public static class FileExists {

        public static bool PersistentFileExists(string subFilePath) {
            Debug.Log($"Checking on: {Path.Combine(Application.persistentDataPath, subFilePath)}");
            return File.Exists(Path.Combine(Application.persistentDataPath, subFilePath));
        }

    }
}