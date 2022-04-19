using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class OtherInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private bool inspectOn;
    private BoxCollider2D box;
    [SerializeField] private GameObject cables;
    void Start()
    {
        inspectOn = false;
        box = GetComponent<BoxCollider2D>();
        DisableSpriteRenderer();
        //cables.SetActive(inspectOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inspect"))
        {
            inspectOn = !inspectOn;
            //this.gameObject.transform.GetChild(2).gameObject.active = inspectOn;
            this.gameObject.transform.GetChild(2).gameObject.SetActive(inspectOn);
            DisableSpriteRenderer();
            //cables.SetActive(inspectOn);
        }
    }

    private void DisableSpriteRenderer()
    {
        for (int i = 0; i < cables.transform.childCount; i++)
        {
            cables.transform.GetChild(i).gameObject.GetComponent<SpriteShapeRenderer>().enabled = inspectOn;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "nextArea")
        {
            //General.fade((int)5);
        }
    }
}
