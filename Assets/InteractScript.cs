using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : PowerScript
{
    protected BoxCollider2D interactCollider;
    private enum Direction { Left, Right, Up, Down }
    protected Transform objectTransform, playerTransform;
    [SerializeField] private Direction directionPlayer;
    protected bool interactable = false;
    [SerializeField] protected bool interact = false;

    // Start is called before the first frame update
    void Start()
    {
        power = false;
    }
    protected IEnumerator InteractWait()
    {
        yield return new WaitForSeconds(0.1f);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckInteractPosition(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckInteractPosition(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.GetChild(0).transform.gameObject.SetActive(false);
            interactable = false;
        }
    }
    private void CheckInteractPosition(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            directionPlayer = (Direction)collision.gameObject.GetComponent<AnimatorController>().direction;
            playerTransform = collision.gameObject.GetComponent<Transform>();

            if (playerTransform.position.x < objectTransform.position.x && directionPlayer == Direction.Right ||
                playerTransform.position.x > objectTransform.position.x && directionPlayer == Direction.Left ||
                playerTransform.position.y < objectTransform.position.y && directionPlayer == Direction.Up ||
                playerTransform.position.y > objectTransform.position.y && directionPlayer == Direction.Down)
            {
                collision.gameObject.transform.GetChild(0).transform.gameObject.SetActive(true);
                interactable = true;
            }
        }

        
    }
}
