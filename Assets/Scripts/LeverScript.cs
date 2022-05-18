using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.GameInput;

public class LeverScript : InteractScript
{
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private int id;
    // Start is called before the first frame update
    void Start()
    {
        interactCollider = GetComponent<BoxCollider2D>();
        objectTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        power = false;
        //UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && InputController.instance.GetButtonDown("Interact"))
        {
            UpdateState();
        }
    }

    void UpdateState()
    {
        power = !power;
        if (power)
        {
            spriteRenderer.sprite = sprites[1];
        }
        else
        {
            spriteRenderer.sprite = sprites[0];
        }
        General.evento_Energia(id);
        InteractWait();
    }
}
