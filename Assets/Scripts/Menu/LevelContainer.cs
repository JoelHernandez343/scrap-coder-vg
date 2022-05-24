// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Game {
    public class LevelContainer : MonoBehaviour {

        [System.Serializable]
        public class Level {

            public Sprite image;
            public string title;
            public string description;
            public string sceneName;

        }

        [SerializeField] public List<Level> levels;

    }
}