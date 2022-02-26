using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InteractCopper : InteractScript
{
    // Start is called before the first frame update
    void Start()
    {
        interactCollider = GetComponent<BoxCollider2D>();
        objectTransform = GetComponent<Transform>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && Input.GetButtonDown("Interact"))
        {
            Programar();
        }
    }

    void Programar()
    {
        SceneManager.LoadScene("NodesScene");
    }
}