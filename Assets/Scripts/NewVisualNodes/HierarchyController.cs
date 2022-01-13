using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyController : MonoBehaviour {

    [SerializeField] int beginZ = 0;

    List<NodeController> nodes = new List<NodeController>();

    public static HierarchyController instance {
        private set;
        get;
    }

    void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void SetOnTop(NodeController controller) {
        controller = FindParent(controller);

        var index = nodes.IndexOf(controller);

        if (index == -1) {
            nodes.Add(controller);
            SetAllZ();
        } else if (index != nodes.Count - 1) {
            nodes.RemoveAt(index);
            nodes.Add(controller);
            SetAllZ();
        }
    }

    public int IndexOf(NodeController controller) {
        return nodes.IndexOf(controller);
    }

    NodeController FindParent(NodeController controller) {
        while (controller.controller != null) {
            controller = controller.controller;
        }

        return controller;
    }

    void SetAllZ() {
        for (var i = beginZ; i < nodes.Count; ++i) {
            var sorter = nodes[i].GetComponent<UnityEngine.Rendering.SortingGroup>();
            sorter.sortingOrder = i;
        }
    }
}
