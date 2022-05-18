using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using SimpleFileBrowser;
using ScrapCoder.Utils;
using ScrapCoder.VisualNodes;

public class openFileButton : MonoBehaviour {

    ButtonController _button;
    ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

    void Start() {
        button.AddListener(() => {
            // OpenFileExample();
            SaveFileExample();
        });
    }

    void OpenFileExample() {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json Files", ".json"));
        FileBrowser.SetDefaultFilter(".json");

        FileBrowser.ShowLoadDialog(
            onSuccess: (paths) => OnSucces(paths[0]),
            onCancel: () => { Debug.Log("Canceled"); },
            pickMode: FileBrowser.PickMode.Files,
            allowMultiSelection: false,
            initialPath: null,
            initialFilename: null,
            title: "Select Folder",
            loadButtonText: "Select"
        );
    }

    public void SaveFileExample() {

        FileBrowser.ShowSaveDialog(
            onSuccess: (paths) => OnSuccesSave(paths[0]),
            onCancel: () => { Debug.Log("Canceled"); },
            pickMode: FileBrowser.PickMode.Files,
            allowMultiSelection: false,
            initialPath: "C:\\",
            initialFilename: "example.json",
            title: "Save As",
            saveButtonText: "Save"
        );
    }

    void OnSucces(string filePath) {

        var example = SaveLoadJson<NodeControllerCustomInformation>.LoadJsonFromFile(filePath);

        Debug.Log($"TextValue: {example.textValue}, SelectedOption: {example.selectedOption}");

    }

    void OnSuccesSave(string filePath) {
        var example = new NodeControllerCustomInformation {
            selectedOption = "hmmmm",
            textValue = "someValueInText"
        };

        SaveLoadJson<NodeControllerCustomInformation>.SaveJsonToFile(data: example, filePath);

    }

}
