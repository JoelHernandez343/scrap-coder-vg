using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.UI;
using ScrapCoder.Interpreter;

public class SelectExecuterTimingDropMenu : MonoBehaviour {
    [SerializeField] DropMenuController dropMenu;

    // Start is called before the first frame update
    void Start() {
        dropMenu.AddListener(() => ChangeExecuterTiming());
    }

    void ChangeExecuterTiming() {
        var option = dropMenu.Value;

        if (option == "Immediately") {
            Executer.instance.SetTiming(ExecuterTiming.Immediately);
        } else if (option == "EverySecond") {
            Executer.instance.SetTiming(ExecuterTiming.EverySecond);
        } else if (option == "EveryThreeSeconds") {
            Executer.instance.SetTiming(ExecuterTiming.EveryThreeSeconds);
        }
    }
}
