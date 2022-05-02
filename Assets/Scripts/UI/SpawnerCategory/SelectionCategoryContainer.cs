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
        [SerializeField] ButtonController returnButton;

        [SerializeField] NodeSpawnController spawnerPrefab;

        // Lazy variables
        NodeTransform _ownTransform;
        public NodeTransform ownTransform => _ownTransform ??= (GetComponent<NodeTransform>() as NodeTransform);

        // State variables
        bool initialized = false;

        List<NodeSpawnController> spawners;

        // Methods
        void Start() {
            Initialize();
        }

        public void Initialize(
            List<NodeSpawnTemplate> spawnersTemplates = null,
            System.Action returnCallback = null
        ) {
            if (initialized) return;

            System.Action dissapearAndReturn = () => {
                returnCallback?.Invoke();
                SetVisible(visible: false, smooth: true);
            };

            spawners = CreateSpawners(
                spawnersTemplates: spawnersTemplates,
                returnCallback: dissapearAndReturn
            );

            var newY = PositionAllSpawners();
            RefreshDimensions(-newY);

            returnButton.AddListener(dissapearAndReturn);

            SetVisible(visible: false, smooth: false);

            initialized = true;
        }

        List<NodeSpawnController> CreateSpawners(List<NodeSpawnTemplate> spawnersTemplates, System.Action returnCallback) {
            return spawnersTemplates?.ConvertAll(t => NodeSpawnController.Create(
                spawnerPrefab: spawnerPrefab,
                parent: children,
                returnCallback: returnCallback,
                template: t
            ));
        }

        int PositionAllSpawners() {
            var lastY = -14;

            spawners?.ForEach(s => {
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

        public void SetVisible(bool visible, bool smooth = false) {
            ownTransform.SetPosition(
                x: visible ? 6 : -(ownTransform.width + 10),
                smooth: smooth
            );
        }

    }
}