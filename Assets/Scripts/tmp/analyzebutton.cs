using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.Interpreter;

public class analyzebutton : MonoBehaviour {

    [SerializeField] ButtonController button;
    Editor editor => InterfaceCanvas.instance.editorVisibiltyManager;

    void Start() {
        button.AddListener(() => {
            if (!editor.isEditorOpenRemotely) {
                Executer.instance.Execute();
            } }
            );
    }

}
