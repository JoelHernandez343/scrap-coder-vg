using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPosition : MonoBehaviour
{
    private Transform[] positions = new Transform[2];

    // Maybe this a possible fix:
    [SerializeField] Transform copperTransform;

    // Start is called before the first frame update
    void Start()
    {
        // positions[0] = transform.GetChild(0).GetComponent<Transform>();
        // positions[1] = transform.GetChild(1).GetComponent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            /*collision.transform.position = positions[0].position;
            GameObject.Find("Copper").transform.position = positions[1].position;
            ResetPositionEvent.newPosition(true);*/
            // ResetPosition(copper, new Vector3...);
        }
    }
}
