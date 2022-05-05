// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.UI {
    public class SelectionController : MonoBehaviour {

        // Editor variables
        [SerializeField] Transform categoriesParent;

        [SerializeField] SpawnerSelectionController categoryPrefab;
        [SerializeField] SpawnerSelectionController categoryWithDeclarationPrefab;

        [SerializeField] NodeController variablePrefab;
        [SerializeField] NodeController arrayPrefab;

        // State variables
        List<SpawnerSelectionController> categoryControllers;

        bool initialized = false;

        // Methods
        void Start() {
            Initialize();
        }

        void Initialize() {
            if (initialized) return;

            var categoryTemplates = new List<SpawnerSelectionTemplate> {
                new SpawnerSelectionTemplate {
                    title = "Control",
                    icon = "control",
                    spawnersTemplates = new List<NodeSpawnTemplate> {
                        new NodeSpawnTemplate {
                            title = "Inicio",
                            nodeToSpawn = NodeType.Begin,
                            selectedIcon = "begin",
                            spawnLimit = 1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Fin",
                            nodeToSpawn = NodeType.End,
                            selectedIcon = "end",
                            spawnLimit = 1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Repetir_si",
                            nodeToSpawn = NodeType.Repeat,
                            selectedIcon = "repeat_simple",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Repetir_n_veces",
                            nodeToSpawn = NodeType.RepeatNTimes,
                            selectedIcon = "repeat_simple",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Si,_entonces,si_no",
                            nodeToSpawn = NodeType.ElseConditional,
                            selectedIcon = "conditional_compund",
                            spawnLimit = -1,
                            symbolName = null
                        },
                    },
                    declarationType = "none"
                },
                new SpawnerSelectionTemplate {
                    title = "Condiciones",
                    icon = "condition",
                    spawnersTemplates = new List<NodeSpawnTemplate> {
                        new NodeSpawnTemplate {
                            title = "Verdadero",
                            nodeToSpawn = NodeType.TrueConstant,
                            selectedIcon = "cond_true",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Falso",
                            nodeToSpawn = NodeType.FalseConstant,
                            selectedIcon = "cond_false",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Condición_Y",
                            nodeToSpawn = NodeType.ConditionAnd,
                            selectedIcon = "cond_double",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Condicion_O",
                            nodeToSpawn = NodeType.ConditionOr,
                            selectedIcon = "cond_double",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Negación",
                            nodeToSpawn = NodeType.ConditionNegation,
                            selectedIcon = "cond_negation",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Comparación_numérica",
                            nodeToSpawn = NodeType.NumericValueComparison,
                            selectedIcon = "cond_double_numeric_values",
                            spawnLimit = -1,
                            symbolName = null
                        },
                    },
                    declarationType = "none"
                },
                new SpawnerSelectionTemplate {
                    title = "Instrucciones",
                    icon = "instructions",
                    spawnersTemplates = new List<NodeSpawnTemplate> {
                        new NodeSpawnTemplate {
                            title = "Caminar",
                            nodeToSpawn = NodeType.Walk,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Girar",
                            nodeToSpawn = NodeType.Rotate,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Interactuar",
                            nodeToSpawn = NodeType.Interact,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Establecer_variable",
                            nodeToSpawn = NodeType.SetVariable,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Agregar_elemento_a_arreglo",
                            nodeToSpawn = NodeType.AddToArray,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Vaciar_arreglo",
                            nodeToSpawn = NodeType.ClearArray,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Eliminar_elemento_de_arreglo",
                            nodeToSpawn = NodeType.RemoveFromArray,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Establecer_elemento_de_arreglo",
                            nodeToSpawn = NodeType.SetValueOfArray,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Insertar_elemento_en_arreglo",
                            nodeToSpawn = NodeType.InsertInArray,
                            selectedIcon = "simple_instruction",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Número",
                            nodeToSpawn = NodeType.NumericValue,
                            selectedIcon = "var_numeric_value",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Operación_numérica",
                            nodeToSpawn = NodeType.NumericOperation,
                            selectedIcon = "var_sum",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Obtener_valor_de_arreglo",
                            nodeToSpawn = NodeType.ValueOfArray,
                            selectedIcon = "var_numeric_value",
                            spawnLimit = -1,
                            symbolName = null
                        },
                        new NodeSpawnTemplate {
                            title = "Obtener_longitud_de_arreglo",
                            nodeToSpawn = NodeType.LengthOfArray,
                            selectedIcon = "var_numeric_value",
                            spawnLimit = -1,
                            symbolName = null
                        },
                    },
                    declarationType = "none"
                },
                new SpawnerSelectionTemplate {
                    title = "Variables",
                    icon = "variable",
                    declarationType = "variable",
                    spawnLimit = 10,
                    declaredPrefix = "var_",
                    spawnerIcon = "var_simple",
                },
                new SpawnerSelectionTemplate {
                    title = "Arreglos",
                    icon = "array",
                    declarationType = "array",
                    spawnLimit = 10,
                    declaredPrefix = "array_",
                    spawnerIcon = "array_simple",
                },
            };

            categoryControllers = categoryTemplates.ConvertAll(
                template => SpawnerSelectionController.Create(
                    prefab: template.declarationType == "none"
                        ? categoryPrefab
                        : categoryWithDeclarationPrefab,
                    parent: categoriesParent,
                    template: template,
                    selectionController: this,
                    prefabToSpawn: template.declarationType == "none"
                        ? null
                        : template.declarationType == "variable"
                        ? variablePrefab
                        : arrayPrefab
                )
            );

            LocateButtons();

            initialized = true;
        }

        void LocateButtons() {
            var y = 10;

            categoryControllers.ForEach(c => y += c.LocateButton(y) + 10);
        }

        public void HideAllButtons() {
            categoryControllers.ForEach(c => c.SetButtonVisible(visible: false));
        }

        public void ShowAllButtons() {
            categoryControllers.ForEach(c => c.SetButtonVisible(visible: true));
        }

    }
}