using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform movePoint;
    [SerializeField] private enum Direction { Left, Up, Right, Down, None }
    [SerializeField] private enum Action { Walk, Rotate, Interact, None }
    [SerializeField] private Direction dirMovement, dirFacing;
    [SerializeField] private Action action;
    [SerializeField] private int steps;

    private int rotateAux;

    [SerializeField] private int rotate;
    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        action = Action.None;
        dirMovement = Direction.Down;
        //move = false;
        rotate = 0;
        rotateAux = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (action != Action.None)
        {
            switch ((int)action)
            {
                case 0:
                    dirMovement = dirFacing;
                    Move(dirMovement);
                    break;
                case 1:
                    Rotate(dirFacing);
                    //dirMovement = dirFacing;
                    break;
                case 2:
                    break;
            }
            dirMovement = Direction.None;
            //dirMovement = Direction.None;
            //move = false;
        }
        /*if(steps > 0)
        {
            move = MoveRobot(steps);
            steps = 0;
        }*/
    }

    void Move(Direction dir)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            //move = false;
            switch ((int)dir)
            {
                case 0:
                    movePoint.position += new Vector3(-1, 0, 0);
                    break;
                case 1:
                    movePoint.position += new Vector3(0, 1, 0);
                    break;
                case 2:
                    movePoint.position += new Vector3(1, 0, 0);
                    break;
                case 3:
                    movePoint.position += new Vector3(0, -1, 0);
                    break;
                default:
                    break;
            }
        }

    }
    void Rotate(Direction dirF)
    {
        rotateAux = (int)dirF + rotate;
        if(rotateAux == -1)
        {
            rotateAux = 3;
        }else if (rotateAux > 3)
        {
            rotateAux = 0;
        }
        dirFacing = (Direction)rotateAux;
        action = Action.None;
    }
        /*  This function determines how many steps the Robot takes,
      the direction is determined by the direction the robot is facing*/

    /*private void Update()
    {
        Action()
    }

    void Action(int action)
    {
        switch (action)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    

    void Walk()
    {

    }*/
}
