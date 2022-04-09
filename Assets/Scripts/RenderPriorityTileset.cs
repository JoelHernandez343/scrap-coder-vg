using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RenderPriorityTileset : MonoBehaviour
{
    private bool cambioPrioridad = false;
    private GameObject Player;
    private TilemapRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<TilemapRenderer>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (cambioPrioridad)
        {
            determinarPrioridad();
        }

    }

    private void determinarPrioridad()
    {
        Vector2 direccionPlayer = Player.transform.position - transform.position;
        SpriteRenderer spriteRendererPlayer = Player.GetComponent<SpriteRenderer>();
        if (direccionPlayer.x > -2 && direccionPlayer.y > 0 || direccionPlayer.x > -0.2 && direccionPlayer.y > 0)
        { //Player arriba
            spriteRenderer.sortingOrder = 2;
            spriteRendererPlayer.sortingOrder = 1;
        }
        else
        { //Player debajo
            spriteRenderer.sortingOrder = 1;
            spriteRendererPlayer.sortingOrder = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player = collision.gameObject;
            cambioPrioridad = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            cambioPrioridad = false;
        }
    }
}