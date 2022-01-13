using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Alberto García
/* Este es el código que se encarga del movivimiento del personaje */
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float horizontalMovement;
    [SerializeField] private float verticalMovement;
    [SerializeField] private float movementSpeed;
    //private Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        //transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement() /* Esta función recupera la entrada */
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        if (horizontalMovement == 1)        // Derecha
        {
            transform.position = new Vector3(transform.position.x + movementSpeed, transform.position.y, transform.position.z);
        }
        else if (horizontalMovement == -1)   // Izquierda
        {
            transform.position = new Vector3(transform.position.x - movementSpeed, transform.position.y, transform.position.z);
        }
        if (verticalMovement == 1)           // Arriba
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + movementSpeed, transform.position.z);
        }
        else if (verticalMovement == -1)     // Abajo
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - movementSpeed, transform.position.z);
        }
    }

}
