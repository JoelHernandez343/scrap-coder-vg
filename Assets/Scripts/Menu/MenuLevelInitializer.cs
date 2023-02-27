// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

using ScrapCoder.VisualNodes;
using ScrapCoder.Game;
using ScrapCoder.Utils;

using SimpleFileBrowser;

namespace ScrapCoder.UI {
    public class MenuLevelInitializer : MonoBehaviour {

        // Editor variables
        [SerializeField] LevelLoader levelContainer;

        // Methods
        void Start() {

            levelContainer.CreateLevelDataIfNotExists();

        }

    }
}