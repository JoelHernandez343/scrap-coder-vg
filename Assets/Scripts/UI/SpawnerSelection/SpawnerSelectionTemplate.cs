// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SpawnerSelectionTemplate {

        public string title;
        public string icon;
        public List<NodeSpawnTemplate> spawnersTemplates;

        public string declarationType;

        public int spawnLimit;
        public string declaredPrefix;
        public string spawnerIcon;

        public int declarationLimit;

    }
}