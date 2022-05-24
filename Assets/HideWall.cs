using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideWall : MonoBehaviour
{
    private TilemapRenderer tileMapRenderer;
    private BoxCollider2D boxCollider;
    public bool bolean, playerInside;
    [SerializeField] private int idWall;
    void Start()
    {
        tileMapRenderer = GetComponent<TilemapRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        bolean = true;
    }
    void OnEnable()
    {
        HideWallComs.hide += Message;
    }

    private void OnDestroy()
    {
        HideWallComs.hide -= Message;
    }

   private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerInside = true;
            tileMapRenderer.enabled = false;
            bolean = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInside = false;
            tileMapRenderer.enabled = true;
            bolean = false;
        }
        
    }

    private void Message(int id)
    {
        
        bolean = !bolean;
        if (playerInside)
        {
            tileMapRenderer.enabled = true;
        }
        else
        {
            tileMapRenderer.enabled = bolean;
        }
        
        if (id == idWall)
        {
            
            print("mensaje");
        }
    }
}
