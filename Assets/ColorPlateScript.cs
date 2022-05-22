using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlateScript : MonoBehaviour
{
    [SerializeField] private enum Color { Green, Blue, Oranje, Gray, Brown, Red, None }
    [SerializeField] private Sprite[] colors;
    [SerializeField] private Color id;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setColor((int)id);
    }

    public void setColor(int newId)
    {
        id = (Color)newId;
        spriteRenderer.sprite = colors[(int)id];
    }

   public int getColor()
    {
        return (int)id;
    }
}
