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
                Debug.LogError($"Unable to convert {inputText.Value}");
                return false;
            }
        }

    }

}