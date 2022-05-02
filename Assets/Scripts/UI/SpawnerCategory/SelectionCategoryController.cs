// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionCategoryController : MonoBehaviour {

        // Editor variables
        [SerializeField] SelectionCategoryButton button;
        [SerializeField] SelectionCategoryContainer container;

        // State variables
        bool initialized = false;

        // Methods
        void Start() {
            Initialize(
                spawnersTemplates: new List<NodeSpawnTemplate> {
                    new NodeSpawnTemplate {
                        title = "Inicio",
                        nodeToSpawn = NodeType.Begin,
                        selectedIcon = "begin",
                        spawnLimit = -1,
                        symbolName = null
                    }
                },
                title: "Instrucciones",
                icon: "instructions"
            );
        }

        void Initialize(List<NodeSpawnTemplate> spawnersTemplates, string title, string icon) {
            if (initialized) return;

            ConfigureContainer(spawnersTemplates);
            ConfigureButton(title: title, icon: icon);

            initialized = true;
        }

        void ConfigureContainer(List<NodeSpawnTemplate> spawnersTemplates) {
            container.Initialize(
                spawnersTemplates: spawnersTemplates,
                returnCallback: () => {
                    button.SetVisibleState(state: SelectionCategoryButtonState.HalfVisible);
                }
            );
        }

        void ConfigureButton(string title, string icon) {
            button.Initialize(
                title: title,
                icon: icon,
                callback: () => {
                    container.SetVisible(visible: true, smooth: true);
                }
            );
        }

    }
}