using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : PowerScript
{
    //[SerializeField] public bool power=false;
    [SerializeField] private Sprite[] sprites;
    private BoxCollider2D buttonTrigger;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private int objectsInside;
    [SerializeField] private int id;


    // Start is called before the first frame update
    void Start()
    {
        objectsInside = 0;
        power = false;
        buttonTrigger = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (objectsInside == 0)
            objectsInside++;
        if (!power)
        {
            power = true;
            General.evento_Energia(id);
            spriteRenderer.sprite = sprites[1];
        }
        
        
        //General.evento_Energia(power);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsInside++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInside--;
        if (power)
        {
            if (objectsInside < 1)
            {
                power = false;
                General.evento_Energia(id);
                spriteRenderer.sprite = sprites[0];
            }
        }
       
    }
}
