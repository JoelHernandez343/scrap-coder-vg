using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public class NodeDictionaryController : MonoBehaviour {

        // Internal types
        [System.Serializable]
        struct NodeNameTuple {
            public NodeType type;
            public NodeController prefab;
        }

        // Editor variables
        [SerializeField] List<NodeNameTuple> prefabs;

        // Lazy and other variables
        public static NodeDictionaryController instance {
            private set;
            get;
        }

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        public NodeController this[NodeType type]
            => prefabs.Find(prefab => prefab.type == type).prefab;

    }

}