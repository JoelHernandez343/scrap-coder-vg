using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    // Start is called before the first frame update
    private Image image;
    private bool bol;
    private int fadeSpeed;
    private int count;
    void Start()
    {
        Image image = GetComponent<Image>();
        fadeSpeed = 5;
        // image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
    }
    void OnEnable()
    {
        General.fade += Fade;
    }

    private void OnDestroy()
    {
        General.fade += Fade;
    }

    private void Fade(int speed)
    {
        //fadeSpeed = speed;
        /*if (image.color.a == 0f)//Color(image.color.r, image.color.g, image.color.b, 0f))
        {
            // FadeIn
            bol = true;
        }
        else
        {
            // FadeOut
            bol = false;
        }*/
        bol = true;
        count = 0;
        StartCoroutine("FadeEnumerator");
    }
    public IEnumerator FadeEnumerator()
    {
        if (bol)
        {
            bol = !bol;
            while(count < 1)
            {
                count = (int)((int)image.color.a + fadeSpeed * Time.deltaTime);
                image.color =  new Color(image.color.r, image.color.g, image.color.b, count);
                yield return null;
            }
        }
        else
        {
            bol = !bol;
            while (count > 0)
            {
                count = (int)(image.color.a - fadeSpeed * Time.deltaTime);
                image.color = new Color(image.color.r, image.color.g, image.color.b, count);
                yield return null;
            }
        }
        yield return null;
    }
}


