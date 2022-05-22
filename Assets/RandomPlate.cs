using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ColorPlateScript[] plates;

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
        if(x == 0)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                plates[i].setColor((int)Random.Range(0, 5));
            }
        }
    }
}
