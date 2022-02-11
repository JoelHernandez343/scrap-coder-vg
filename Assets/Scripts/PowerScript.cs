using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public bool power = false;

    public bool GetPower()
    {
        return power;
    }
}
