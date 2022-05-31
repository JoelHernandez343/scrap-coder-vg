// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using Newtonsoft.Json;

using ScrapCoder.VisualNodes;
using ScrapCoder.Game;
using ScrapCoder.Utils;
using ScrapCoder.Interpreter;

namespace ScrapCoder.UI {
    public class ImportNodesButton : MonoBehaviour {
        // Lazy Variables
        ButtonController _button;
        ButtonController button => _button ??= (GetComponent<ButtonController>() as ButtonController);

        // Methods
        void Start() {
            button.AddListener(() => OpenFile());
        }

        void OpenFile() {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Json Files", ".json"));
            FileBrowser.SetDefaultFilter(".json");

            FileBrowser.ShowLoadDialog(
                onSuccess: (paths) => ImportNodes(paths[0]),
                onCancel: () => { Debug.Log("Canceled"); },
                pickMode: FileBrowser.PickMode.Files,
                allowMultiSelection: false,
                initialPath: null,
                initialFilename: null,
                title: "Select Folder",
                loadButtonText: "Select"
            );
        }

        void ImportNodes(string filePath) {
            NodeJsonData nodeJsonData = null;

            try {
                nodeJsonData = SaveLoadJson<NodeJsonData>.LoadJsonFromFile(filePath);
            } catch (JsonException e) {
                Debug.LogError(e, this);

                MessagesController.instance.AddMessage(
                    message: $"Ocurrió un error leyendo el archivo {FileBrowserHelpers.GetFilename(path: filePath)}",
                    type: MessageType.Error
                );

                return;
            }

            var symbolNameChanges = SymbolTable.instance.UpdateSymbolsTemplates(nodeJsonData.symbolTableTemplate);

            nodeJsonData.symbolTableTemplate?.variableTemplates.ForEach(t => {
                InterfaceCanvas.instance.variableDeclareContainer.Declare(
                    symbolName: t.symbolName,
                    smooth: false,
                    template: t
                );
            });

            nodeJsonData.symbolTableTemplate?.arrayTemplates.ForEach(t => {
                InterfaceCanvas.instance.arrayDeclareContainer.Declare(
                    symbolName: t.symbolName,
                    smooth: false,
                    template: t
                );
            });

            nodeJsonData.nodesTuples?.ForEach(tuple => {
                var nodeTree = tuple.nodeTemplate;
                var position = tuple.position;

                if (nodeTree.type == NodeType.Variable || nodeTree.type == NodeType.Array) {
                    nodeTree.symbolName = symbolNameChanges?[nodeTree.symbolName] ?? nodeTree.symbolName;
                }

                var prefab = SymbolTable.instance[nodeTree.symbolName].spawner.prefabToSpawn;

                var realNode = NodeController.Create(
                    prefab: prefab,
                    parent: InterfaceCanvas.instance.workingZone.transform,
                    template: nodeTree,
                    symbolNameChanges: symbolNameChanges
                );

                realNode?.ownTransform.SetScale(
                    x: InterfaceCanvas.NodeScaleFactor,
                    y: InterfaceCanvas.NodeScaleFactor,
                    z: 1
                );

                realNode?.ownTransform.SetPosition(
                    x: position.x,
                    y: position.y
                );
            });
        }
    }
}