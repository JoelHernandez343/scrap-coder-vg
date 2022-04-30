// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.Interpreter;

public class CleanWorkingZoneButton : MonoBehaviour {

    [SerializeField] ButtonController button;

    void Start() {

        button.AddListener(() => SymbolTable.instance.CleanReferencesWihoutParent());

    }
}
