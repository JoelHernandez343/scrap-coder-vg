using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private int powerSources = 1;
    [SerializeField] private bool[] source;
    [SerializeField] private bool power;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        powerSources = source.Length;
        power = true;

    }

    void OnEnable()
    {
        General.evento_Energia += OpenOrClose;
    }

    private void OnDestroy()
    {
        General.evento_Energia -= OpenOrClose;
    }

    // Update is called once per frame
    /*void FixedUpdate()
    {
        power = true;
        for(int i=0; i < powerSources; i++)
            power = power & activador[i].GetComponent<PowerScript>().power;
        if (power)
        {
            anim.SetBool("Power", true);
        }
        else
        {
            anim.SetBool("Power", false);
        }

    }*/

    private void OpenOrClose(int id)
    {
        power = true;
        source[id] = !source[id];
        for (int i = 0; i < powerSources; i++)
            power = power & source[i];
        if (power)
        {
            anim.SetBool("Power", true);
        }
        else
        {
            anim.SetBool("Power", false);
        }
    }
}
