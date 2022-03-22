using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideWall : MonoBehaviour
{
    private TilemapRenderer renderer;
    private BoxCollider2D collider;
    public bool bolean, playerInside;
    [SerializeField] private int idWall;
    void Start()
    {
        renderer = GetComponent<TilemapRenderer>();
        collider = GetComponent<BoxCollider2D>();
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

   /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerInside = true;
            renderer.enabled = false;
            bolean = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInside = false;
            renderer.enabled = true;
            bolean = false;
        }
        
    }*/

    private void Message(int id)
    {
        
        bolean = !bolean;
        if (playerInside)
        {
            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = bolean;
        }
        
        if (id == idWall)
        {
            
            print("mensaje");
        }
    }
}
