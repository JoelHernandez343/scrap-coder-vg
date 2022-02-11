using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    // Start is called before the first frame update
    private enum Direction { Left, Right, Up, Down }
    [SerializeField] private Direction direction, change;
    [SerializeField] private float speed;
    private int movementHorizontal, movementVertical;
    private Transform[] trans;
    private SpriteRenderer spriteRend;
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        //speed = 0.1f;
        //direction = Direction.Right;
        //change = direction;
        movementVertical = 0;
        movementHorizontal = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(direction != change)
        {
            switch ((int)direction)
            {
                case 0:     // Left
                    movementVertical = 0;
                    movementHorizontal = -1;
                    spriteRend.flipX = true;
                    spriteRend.flipY = false;
                    break;
                case 1:     // Right
                    movementVertical = 0;
                    movementHorizontal = 1;
                    spriteRend.flipX = false;
                    spriteRend.flipY = false;
                    break;
                case 2:     // Up
                    movementVertical = 1;
                    movementHorizontal = 0;
                    break;
                default:    // Down
                    movementVertical = -1;
                    movementHorizontal = 0;
                    break;
            }
            change = direction;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Transform trans = collision.gameObject.GetComponent<Transform>();
        trans.position = new Vector3(trans.position.x + movementHorizontal*speed, trans.position.y + movementVertical*speed, transform.position.z);

    }
}
