using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetController : MonoBehaviour
{
    //[SerializeField] private Transform BelPosition, CopperPosition;
    [SerializeField] private Vector2 BelPosition, CopperPosition;
    [SerializeField] private GameObject Bel, Copper;

    // Start is called before the first frame update
    void Start()
    {
        //UpdatePosition(true);
    }

    void OnEnable()
    {
        //ResetPositionEvent.reset += ResetPosition;
        ResetPositionEvent.newPosition += UpdatePosition;
    }

    private void OnDestroy()
    {
        //ResetPositionEvent.reset -= ResetPosition;
        ResetPositionEvent.newPosition = UpdatePosition;
    }

    // Update is called once per frame
    void Update()
    {
        //ResetPosition(true);
        if (Input.GetButtonDown("Reset") /* || dies */)
        {
            ResetPosition(true);
        }
    }

    // Called every time the player crosses to anothe are with a new respawn position
    private void UpdatePosition(bool a)
    {
        BelPosition = new Vector2(Bel.transform.position.x, Bel.transform.position.y);
        CopperPosition = new Vector2(Copper.transform.position.x, Copper.transform.position.y);
    }

    // Called every time the player uses "Reset" or loses
    private void ResetPosition(bool b)
    {
        Debug.Log("reset");
        //Bel.transform.position.Set(BelPosition.x, BelPosition.y, Bel.transform.position.z); //Transform.position(Bel.transform.position.x, Bel.transform.position.y, Bel.transform.position.z);
        Copper.transform.position.Set(CopperPosition.x, CopperPosition.y, Copper.transform.position.z);
        GameObject.Find("Player").transform.position.Set(BelPosition.x, BelPosition.y, Bel.transform.position.z);
    }
}
