using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] Animator anim;
    public enum Direction {Left, Right, Up, Down}
    private enum Action {Walk, Stand}
    [SerializeField] public Direction direction;
    [SerializeField] private Action action;
    private float movementX, movementY;

    void Start()
    {
        anim = GetComponent<Animator>();
        direction = Direction.Right;
        anim.SetInteger("Direction", (int)direction);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        movementY = Input.GetAxisRaw("Vertical");

        if(movementX != 0 || movementY != 0){
            action = Action.Walk;
        }

        if(movementX > 0){ // Looking Right
            direction = Direction.Right;
        }else if(movementX < 0){ // Looking Left
            direction = Direction.Left;
        }else if(movementY > 0){ // Looking Up
            direction = Direction.Up;
        }else if(movementY < 0){ // Looking Down
            direction = Direction.Down;
        }else if(movementX == 0 && movementY == 0){
            action = Action.Stand;
        }

        anim.SetInteger("Action", (int)action);
        anim.SetInteger("Direction", (int)direction);
    }
}
