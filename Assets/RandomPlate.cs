using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomPlate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ColorPlateScript[] plates;
    [SerializeField] private string sequence;
    [SerializeField] public int[] n  = new int[5];
    [SerializeField] private int id;
    int x;

    void Start()
    {
        
    }
    void OnEnable()
    {
        General.evento_Energia += Randomize;
    }

    private void OnDestroy()
    {
        General.evento_Energia -= Randomize;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Randomize(int x)
    {
        if(x == id)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                x = (int)UnityEngine.Random.Range(0, 5);
                plates[i].setColor(x);
                n[i] = x;
            }
        }
        Array.Sort(n);
    }
}
