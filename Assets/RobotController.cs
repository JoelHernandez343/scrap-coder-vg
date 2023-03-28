using ScrapCoder.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    [SerializeField] private Transform movePoint;
    private Vector3 previousMovePoint;

    [SerializeField] private enum Direction { Left, Up, Right, Down, None }
    [SerializeField] private Direction dirMovement, dirFacing;
    [SerializeField] private Actions action;
    [SerializeField] private int steps;
    [SerializeField] private enum Color { Green, Blue, Oranje, Gray, Brown, Red, None }
    [SerializeField] private Color color;
    [SerializeField] private bool moving, panelInteract;
    private Animator anim;
    private Rigidbody2D rb;
    private int rotateAux;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movePoint.parent = null;
        dirMovement = Direction.Down;
        moving = false;
        rotateAux = 0;
        color = Color.None;
        panelInteract = false;
        AnimationSet();

        previousMovePoint = movePoint.position;
    }

    private void Awake()
    {
        SendInstruction.sendInstruction += getInstruction;
        action = Actions.None;
    }

    private void OnDestroy()
    {
        SendInstruction.sendInstruction -= getInstruction;
    }
    // Update is called once per frame
    void Update() {
        if (action == Actions.None) {
            AnimationSet();
            return;
        }

        if (action == Actions.Walk) {
            dirMovement = dirFacing;
            AnimationSet();

            if (!moving) {
                MovePoint();
                moving = true;
            }
            Move(dirMovement);
        } else if (action == Actions.RotateLeft) {
            Rotate(rotate: -1);
        } else if (action == Actions.RotateRight) {
            Rotate(rotate: 1);
        } else if (action == Actions.Scan) {
            ScanColor();
        } else if (action == Actions.Zero || action == Actions.One || action == Actions.Two || action == Actions.Three || action == Actions.Four || action == Actions.Five || action == Actions.Six) {
            Panel();
        } else if (action == Actions.StopSignal) { 
            if (moving) {
                ResetMovement();
                action = Actions.None;
                finishAction();
            }
        }

        dirMovement = Direction.None;
        AnimationSet();
    }

    void Move(Direction dir)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            moving = false;
            action = Actions.None;
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
            previousMovePoint = movePoint.position;

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

    void ResetMovement() { 
        transform.position = previousMovePoint;
        movePoint.position = previousMovePoint;
    }

    void Rotate(int rotate)
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
        action = Actions.None;
        SendInstruction.finishInstruction(null);
    }

    private void AnimationSet()
    {
        anim.SetInteger("action", (int)action);
        anim.SetInteger("dir", (int)dirFacing);
    }

    private void getInstruction(int actionReceived)
    {
        if (System.Enum.IsDefined(typeof(Actions), actionReceived)){
            action = (Actions)actionReceived;
            Debug.Log($"[Robot] Instruction received: {(Actions)actionReceived}");
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
        action = Actions.None;
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
            action = Actions.None;
            StartCoroutine(Wait1());
            /*action = Action.None;
            SendInstruction.finishInstruction(0);
            return 0;*/
            return 0;
        }
        action = Actions.None;
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

