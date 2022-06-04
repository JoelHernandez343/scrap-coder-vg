using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ScrapCoder.GameInput;

using TMPro;


public class Cinematic : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string[] texto;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image image;
    [SerializeField] TextMeshProUGUI t;
    private int i = 0;
    
    void Start()
    {
        image.sprite = sprites[0];
        t.text = texto[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i < 10)
                {
                i++;
                    image.sprite = sprites[i];
                    t.text = texto[i];
            }
            else
            {
                SceneManager.LoadScene("Level 1-1");
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Level 1-1");
    }
}
