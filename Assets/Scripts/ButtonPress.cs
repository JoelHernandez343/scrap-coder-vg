using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : PowerScript
{
    //[SerializeField] public bool power=false;
    [SerializeField] private Sprite[] sprites;
    private BoxCollider2D buttonTrigger;
    private SpriteRenderer spriteRenderer;
    

    // Start is called before the first frame update
    void Start()
    {
        power = false;
        buttonTrigger = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        power = true;
        spriteRenderer.sprite = sprites[1];
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        power = false;
        spriteRenderer.sprite = sprites[0];
    }

}
