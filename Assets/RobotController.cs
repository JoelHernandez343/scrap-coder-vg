using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform movePoint;
    [SerializeField] private enum Direction { Left, Up, Right, Down, None }
    [SerializeField] private enum Action { Walk, RotateLeft, RotateRight, Scan, Interact, None, Zero, One, Two, Three, Four, Five, Six }
    [SerializeField] private Direction dirMovement, dirFacing;
    [SerializeField] private Action action;
    [SerializeField] private int steps;
    [SerializeField] private enum Color { Green, Blue, Oranje, Gray, Brown, Red, None }
    [SerializeField] private Color color;
    [SerializeField] private bool moving, panelInteract;
    private Animator anim;
    private Rigidbody2D rb;
    private int rotateAux;

    [SerializeField] private int rotate;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movePoint.parent = null;
        dirMovement = Direction.Down;
        moving = false;
        rotate = 0;
        rotateAux = 0;
        color = Color.None;
        panelInteract = false;
        AnimationSet();
    }

    private void Awake()
    {
        SendInstruction.sendInstruction += getInstruction;
        action = Action.None;
    }

    private void OnDestroy()
    {
        SendInstruction.sendInstruction -= getInstruction;
    }
    // Update is called once per frame
    void Update()
    {
        //print((int)action);
        
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
                case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                    Panel();
                    break;
            }
            dirMovement = Direction.None;
        }
        /*if(rb.velocity != new Vector2(0,0) && action == Action.None)
        {
            AnimationSet();
        }else if(rb.velocity == new Vector2(0, 0) && action == Action.None)
        {
            AnimationSet();
        }*/
        AnimationSet();
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
        SendInstruction.finishInstruction(null);
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
        SendInstruction.finishInstruction(null);
    }

    private void AnimationSet()
    {
        anim.SetInteger("action", (int)action);
        anim.SetInteger("dir", (int)dirFacing);
    }

    private void getInstruction(int actionReceived)
    {
        if (System.Enum.IsDefined(typeof(Action), actionReceived)){
            action = (Action)actionReceived;
            Debug.Log($"[Robot] Instruction received: {(Action)actionReceived}");
        } else {
            Debug.LogError($"[Robot] Invalid enum index of action: {actionReceived}");
        }
        
    }

    private void ScanColor()
    {
        /*if(color == Color.None)
        {
            SendInstruction.finishInstruction(-1);
        }
        else
        {
            SendInstruction.finishInstruction((int)color);
        }*/
        SendInstruction.finishInstruction((int)color);
        action = Action.None;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ColorPlate")
        {
            color = (Color)collision.GetComponent<ColorPlateScript>().getColor();
        }
        else if (collision.tag == "Panel")
        {
            panelInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ColorPlate" && color == (Color)collision.GetComponent<ColorPlateScript>().getColor() && color != (Color)collision.GetComponent<ColorPlateScript>().getColor())
        {
            color = Color.None;
        }else if(collision.tag == "Panel")
        {
            panelInteract = false;
        }
    }

    private int Panel()
    {
        if (panelInteract)
        {
            PanelEvent.sendNumber((int)action);
            action = Action.None;
            StartCoroutine(Wait1());
            /*action = Action.None;
            SendInstruction.finishInstruction(0);
            return 0;*/
            return 0;
        }
        action = Action.None;
        SendInstruction.finishInstruction(-1);
        return -1;
    }

    IEnumerator Wait1()
    {
        yield return new WaitForSeconds(1);
        SendInstruction.finishInstruction(0);
        
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

