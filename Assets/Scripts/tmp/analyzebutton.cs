using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.Interpreter;

public class analyzebutton : MonoBehaviour {

    [SerializeField] ButtonController button;

    void Start() {
        button.AddListener(() => Executer.instance.Execute());
    }

}
