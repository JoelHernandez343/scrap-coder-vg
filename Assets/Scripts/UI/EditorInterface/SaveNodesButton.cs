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
    public class SaveNodesButton : MonoBehaviour {
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

        void OnSuccesSave(string filePath) {
            var dataToSave = new NodeJsonData {
                symbolTableTemplate = SymbolTable.instance.GetSymbolTableTemplate(),
                nodesTuples = SymbolTable.instance.GetNodesWithoutParent()
                    .ConvertAll(n => new NodeJsonData.NodePositionTuple {
                        position = new NodeJsonData.SerializablePosition {
                            x = n.ownTransform.x,
                            y = n.ownTransform.y
                        },
                        nodeTemplate = n.GetTemplate()
                    })
                    .FindAll(tuple => tuple.nodeTemplate != null)
            };

            if (
                dataToSave.symbolTableTemplate.arrayTemplates.Count == 0 &&
                dataToSave.symbolTableTemplate.variableTemplates.Count == 0 &&
                dataToSave.nodesTuples.Count == 0
            ) {
                MessagesController.instance.AddMessage(
                    message: $"No hay información para guardar, recuerda que ni Inicio ni Fin son guardados",
                    status: MessageStatus.Error
                );

                return;
            }

            SaveLoadJson<NodeJsonData>.SaveJsonToFile(data: dataToSave, filePath);

            MessagesController.instance.AddMessage(
                message: "Nodos guardados de forma exitosa.",
                status: MessageStatus.Normal
            );
        }
    }
}