using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class CableScript : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteShapeRenderer spriteShapeRenderer;
    [SerializeField] private Material[] materials1 = new Material[2];
    private Material[] materials2 = new Material[2];
    //[SerializeField] Color color1, color2;



    [SerializeField] private bool power; 
    [SerializeField] private int id;

    void Start()
    {
        /*color1 = new Color(0.17f, 0.56f, 0.3f, 1f);
        color2 = new Color(0.82f, 1f, 0.88f, 1f);*/

        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        power = false;
        General.evento_Energia += TurnOnOff;
    }
    
    /*private void Awake()
    {
        General.evento_Energia += TurnOnOff;
    }*/

    private void OnDestroy()
    {
        General.evento_Energia -= TurnOnOff;
    }

    private void TurnOnOff(int id)
    {
        if(id == this.id)
        {

            power = !power;
            if (power)
            {
                //renderer.color = color2;
                materials2[0] = materials1[0];
                materials2[1] = materials1[0];
                spriteShapeRenderer.materials = materials2;
            }
            else
            {
                //renderer.color = color1;
                materials2[0] = materials1[1];
                materials2[1] = materials1[1];
                spriteShapeRenderer.materials = materials2;
            }
        }
        
    }
}
