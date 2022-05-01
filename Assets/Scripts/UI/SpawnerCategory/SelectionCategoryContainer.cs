// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionCategoryContainer : MonoBehaviour {

        // Editor variables
        [SerializeField] ScrollBarController scrollBar;
        [SerializeField] NodeTransform content;
        [SerializeField] Transform children;

        [SerializeField] NodeSpawnController spawnerPrefab;

        [SerializeField] List<NodeSpawnTemplate> templates;


        // State variables
        bool initialized = false;

        List<NodeSpawnController> spawners;

        // Methods
        void Start() {
            Initialize();
        }

        public void Initialize() {
            if (initialized) return;

            spawners = CreateSpawners();
            var newY = PositionAllSpawners();
            RefreshDimensions(-newY);

            initialized = true;
        }

        List<NodeSpawnController> CreateSpawners() {
            return templates.ConvertAll(t => NodeSpawnController.Create(
                spawnerPrefab: spawnerPrefab,
                parent: children,
                template: t
            ));
        }

        int PositionAllSpawners() {
            var lastY = -14;

            spawners.ForEach(s => {
                s.ownTransform.SetPosition(x: 18, y: lastY);
                lastY -= s.ownTransform.height + 10;
            });

            return lastY - 14;
        }

        void RefreshDimensions(int newY) {
            if (newY > content.height) {
                content.Expand(dy: newY - content.height);
                scrollBar.RefreshSlider();
            }

        }

    }
}