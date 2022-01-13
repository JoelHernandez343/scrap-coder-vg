using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : InteractScript
{
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        interactCollider = GetComponent<BoxCollider2D>();
        objectTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        power = true;
        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && Input.GetButtonDown("Interact"))
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
        InteractWait();
    }
}
