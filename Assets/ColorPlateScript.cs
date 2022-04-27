using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlateScript : MonoBehaviour
{
    [SerializeField] private enum Color { Green, Blue, Oranje, Gray, Brown, Red, None }
    [SerializeField] private Sprite[] colors;
    [SerializeField] private Color id;

    private SpriteRenderer renderer;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        setColor((int)id);
    }

    public void setColor(int newId)
    {
        id = (Color)newId;
        renderer.sprite = colors[(int)id];
    }

   public int getColor()
    {
        return (int)id;
    }
}
