using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.Game {
    public class LocalLevelManager : MonoBehaviour {

        // Editor variables
        [SerializeField] List<TextAsset> puzzleNodesTemplates;

        // State variables
        int currentLevel;
        bool initialize = false;

        // Methods
        void Start() {
            Initialize();
        }

        public void Initialize(int currentLevel = 0) {
            if (initialize) return;

            this.currentLevel = currentLevel;

            ChangeToLevel(this.currentLevel);

            initialize = true;
        }

        public void ChangeToLevel(int level) {
            // Configuring menu of editor
            InterfaceCanvas.instance.selectionMenus.Initialize(
                selectionTemplates: Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpawnerSelectionTemplate>>(
                    puzzleNodesTemplates[level].text
                )
            );
        }


    }
}