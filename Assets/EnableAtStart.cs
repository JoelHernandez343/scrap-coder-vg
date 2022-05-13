using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAtStart : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Awake()
    {
        (transform.GetChild(0).gameObject).SetActive(true);
    }
}
