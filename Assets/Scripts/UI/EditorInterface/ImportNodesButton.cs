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
                    message: $"Ocurrió un error leyendo el archivo {FileBrowserHelpers.GetFilename(path: filePath)}.",
                    type: MessageType.Error
                );

                return;
            }

            var symbolNameChanges = SymbolTable.instance.UpdateSymbolsTemplates(nodeJsonData.symbolTableTemplate);

            if (!DeclareSymbols(
                    symbolTemplates: nodeJsonData.symbolTableTemplate?.variableTemplates,
                    declareContainer: InterfaceCanvas.instance.variableDeclareContainer
                )
            ) {
                MessagesController.instance.AddMessage(
                    message: $"Ocurrió un error declarando las variables.",
                    type: MessageType.Error
                );

                return;
            }

            if (!DeclareSymbols(
                   symbolTemplates: nodeJsonData.symbolTableTemplate?.arrayTemplates,
                   declareContainer: InterfaceCanvas.instance.arrayDeclareContainer
               )
            ) {
                MessagesController.instance.AddMessage(
                    message: $"Ocurrió un error declarando los arreglos.",
                    type: MessageType.Error
                );

                return;
            }

            if (!InstantiateNodes(nodesTuples: nodeJsonData.nodesTuples, symbolNameChanges: symbolNameChanges)) {
                MessagesController.instance.AddMessage(
                    message: $"Ocurrió un error colocando los nodos.",
                    type: MessageType.Error
                );

                return;
            }

            MessagesController.instance.AddMessage(
                message: "Importación exitosa :)",
                type: MessageType.Normal
            );
        }

        bool DeclareSymbols(List<SymbolTemplate> symbolTemplates, DeclareContainer declareContainer) {
            if (symbolTemplates == null || symbolTemplates.Count == 0) return true;

            foreach (var symbolTemplate in symbolTemplates) {
                var declared = declareContainer?.Declare(
                    symbolName: symbolTemplate.symbolName,
                    smooth: false,
                    template: symbolTemplate
                );

                if (declared != true) return false;
            }

            return true;
        }

        bool InstantiateNodes(List<NodeJsonData.NodePositionTuple> nodesTuples, Dictionary<string, string> symbolNameChanges) {
            if (nodesTuples == null) return true;

            foreach (var tuple in nodesTuples) {
                var nodeTree = tuple.nodeTemplate;
                var position = tuple.position;

                if (nodeTree.type == NodeType.Variable || nodeTree.type == NodeType.Array) {
                    nodeTree.symbolName = symbolNameChanges?[nodeTree.symbolName] ?? nodeTree.symbolName;
                }

                var prefab = SymbolTable.instance[nodeTree.symbolName]?.spawner.prefabToSpawn;

                if (prefab == null) {
                    MessagesController.instance.AddMessage(
                        message: $"El nodo {nodeTree.symbolName} no está disponible.",
                        type: MessageType.Error
                    );

                    return false;
                }

                var realNode = NodeController.Create(
                    prefab: prefab,
                    parent: InterfaceCanvas.instance.workingZone.transform,
                    template: nodeTree,
                    symbolNameChanges: symbolNameChanges
                );

                if (realNode == null) return false;

                realNode.ownTransform.SetScale(
                    x: InterfaceCanvas.NodeScaleFactor,
                    y: InterfaceCanvas.NodeScaleFactor,
                    z: 1
                );

                realNode.ownTransform.SetPosition(
                    x: position.x,
                    y: position.y
                );
            }

            return true;
        }
    }
}