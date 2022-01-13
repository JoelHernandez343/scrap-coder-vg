using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSoundScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image[] Bar;
    private int[] volume;
    void Start()
    {
        volume = new int[3];
        for (int i = 0; i < 3; i++)
            volume[i] = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
