using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private int powerSources = 1, id;
    [SerializeField] private bool[] source;
    [SerializeField] private bool power, crops;
    private Animator anim;
    [SerializeField] private BoxCollider2D coll;
    [SerializeField] private AudioClip[] audios;
    AudioSource sourceAud;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        powerSources = source.Length;
        
        sourceAud = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        power = true;
        General.evento_Energia += OpenOrClose;
    }

    private void OnDestroy()
    {
        General.evento_Energia -= OpenOrClose;
    }

    private void OpenOrClose(int id)
    {
        power = true;
        source[id] = !source[id];
        for (int i = 0; i < powerSources; i++)
            power = power & source[i];
        if (power)
        {
            sourceAud.clip = audios[0];
            anim.SetBool("Power", true);
            if(crops)
                anim.SetBool("Crop", true);
            coll.enabled = false;
            HideWallComs.hide(id);
        }
        else
        {
            sourceAud.clip = audios[1];
            anim.SetBool("Power", false);
            if (crops)
                anim.SetBool("Crop", false);
            coll.enabled = true;
            HideWallComs.hide(id);
        }
        sourceAud.Play();
    }
}
