using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;

namespace ScrapCoder.VisualNodes {

    public class NumberValueAnalyzer : MonoBehaviour, INodeAnalyzer {

        // Editor variables
        [SerializeField] InputText inputText;

        // Methods
        bool INodeAnalyzer.Analyze() {
            try {
                var number = System.Int32.Parse(inputText.Value);
                return true;
            } catch (System.FormatException) {
                MessagesController.instance.AddMessage(
                    message: $"No es posible convertir en número el valor {inputText.Value}.",
                    status: MessageStatus.Error
                );
                return false;
            }
        }

    }

}