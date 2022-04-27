using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform movePoint;
    [SerializeField] private enum Direction { Left, Up, Right, Down, None }
    [SerializeField] private enum Action { Walk, RotateLeft, RotateRight, Scan, Interact, None }
    [SerializeField] private Direction dirMovement, dirFacing;
    [SerializeField] private Action action;
    [SerializeField] private int steps;
    [SerializeField] private enum Color { Green, Blue, Oranje, Gray, Brown, Red, None }
    [SerializeField] private Color color;
    private bool moving;
    private Animator anim;

    private int rotateAux;

    [SerializeField] private int rotate;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        movePoint.parent = null;
        dirMovement = Direction.Down;
        moving = false;
        rotate = 0;
        rotateAux = 0;
        color = Color.None;
    }

    private void Awake()
    {
        SendInstruction.sendInstruction += getInstruction;
        action = Action.None;
        print("suscribed");
    }

    private void OnDestroy()
    {
        SendInstruction.sendInstruction -= getInstruction;
    }
    // Update is called once per frame
    void Update()
    {
        //print((int)action);
        AnimationSet();
        if (action != Action.None)
        {
            switch ((int)action)
            {
                case 0: // Walk
                    /* First, he make sure the movement Direction is equals to the direction the robot is facing,
                       then we check if the robot is moving or not, if not, move the point, now the robot will
                       start moving and we will no longer move the point until moving is false again */
                    // print("Caminar");
                    dirMovement = dirFacing;
                    AnimationSet();
                    if (!moving)
                    {
                        MovePoint();
                        moving = true;
                    }
                    Move(dirMovement);
                    break;
                case 1:
                    rotate = -1;
                    Rotate();
                    break;
                case 2:
                    rotate = 1;
                    Rotate();
                    break;
                case 3:
                    ScanColor();
                    break;
            }
            dirMovement = Direction.None;
        }
    }

    void Move(Direction dir)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            moving = false;
            action = Action.None;
            finishAction();
        }
    }

    private void finishAction()
    {
        SendInstruction.finishInstruction(1);
    }

    void MovePoint()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            switch ((int)dirMovement)
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
    void Rotate()
    {
        rotateAux = (int)dirFacing + rotate;
        if (rotateAux == -1)
        {
            rotateAux = 3;
        } else if (rotateAux > 3)
        {
            rotateAux = 0;
        }
        dirFacing = (Direction)rotateAux;
        action = Action.None;
        SendInstruction.finishInstruction(1);
    }

    private void AnimationSet()
    {
        anim.SetInteger("action", (int)action);
        anim.SetInteger("dir", (int)dirFacing);
    }

    private void getInstruction(int actionR)
    {
        print("Recibiendo " + actionR);
        if(actionR != -1)
        {
            action = (Action)actionR;
            print((int)action);
        }
        else
        {
            print("finished");
        }
        
    }

    private void ScanColor()
    {
        SendInstruction.finishInstruction((int)color);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ColorPlate")
        {
            color = (Color)collision.GetComponent<ColorPlateScript>().getColor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ColorPlate" && color == (Color)collision.GetComponent<ColorPlateScript>().getColor())
        {
            color = Color.None;
        }
    }
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
    }*/

