using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelScript : MonoBehaviour
{
    [SerializeField] private int[] n, x;
    [SerializeField] private RandomPlate random;
    [SerializeField] private Sprite[] numbers;

    [SerializeField] private int i, id;
    private SpriteRenderer renderer;
    private bool t=false;

    // Start is called before the first frame update
    private void Start()
    {
        //General.evento_Energia(id);
        renderer = GetComponent<SpriteRenderer>();
        General.evento_Energia(id);
        x = new int[5];
    }
    void OnEnable()
    {
        PanelEvent.sendNumber += getNumber;
        n = random.n;
        //renderer.sprite = numbers[7];
        i = 0;
    }

    private void OnDestroy()
    {
        PanelEvent.sendNumber -= getNumber;
    }
    // Update is called once per frame
    void Update()
    {
        if(x.SequenceEqual(n) && !t)
        {
            General.evento_Energia(id);
            t = true;
        }
    }

    public void getNumber(int m)
    {
        x[i] = m-6;
        setSprite();
        
        /*if (m == n[i])
        {
            
            
            if (i == n.Length)
            {
                General.evento_Energia(id);
            }
        }
        else
        {
            i = 0;
        }*/
    }

    private void setSprite()
    {
        renderer.sprite = numbers[x[i]];
        i++;
    }

}
