using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dissolve : MonoBehaviour
{
    Image image;
    Material[] material = new Material[3];
    [SerializeField] bool isDissolving = false, appearing, text, bg;
    float fade = 1f;
    [SerializeField] private GameObject text1, father;
    
    

    void Start()
    {
        //for(int i=0 ; i < transform.childCount; i++)
        material[0] = father.GetComponent<Image>().material;
        material[1] = text1.GetComponent<TextMeshProUGUI>().fontSharedMaterial;
        //material[2] = text2.GetComponent<Material>();
        material[0].SetFloat("_Fade", 0);
        material[1].SetFloat("_Fade", 0);
        appearing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDissolving = true;
            //print("hola");
        }

        if (isDissolving)
        {
            fade -= Time.deltaTime*0.75f;
            if(fade <= 0f)
            {
                fade = 0f;
                isDissolving = false;
                GetComponent<Canvas>().enabled = false;
            }
            if(text)
                material[0].SetFloat("_Fade", fade);
            if(bg)
                material[1].SetFloat("_Fade", fade);
        }

        if (appearing)
        {
            GetComponent<Canvas>().enabled = true;
            fade += Time.deltaTime * 0.75f;
            if (fade >= 1f)
            {
                fade = 1f;
                appearing = false;
            }
            if (text)
                material[0].SetFloat("_Fade", fade);
            if (bg)
                material[1].SetFloat("_Fade", fade);
        }
    }
}
