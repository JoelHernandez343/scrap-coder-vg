using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.GameInput;

public class RobotInstructions : MonoBehaviour
{
    private enum Action { Walk, RotateLeft, RotateRight, Interact, None }
    [SerializeField] private Action[] actions;
    [SerializeField] private int cont = 0;
    void Start()
    {
        //newInstruction(0);
    }

    private void Awake()
    {
        SendInstruction.finishInstruction += newInstruction;
    }

    private void Update()
    {
        if (InputController.instance.GetButtonDown("Wistle"))
        {
            if (cont > 0)
            {
                cont = -1;
            }
            else
            {
                cont = 0;
                newInstruction(0);
            }
        }
    }
    private void OnDestroy()
    {
        SendInstruction.finishInstruction -= newInstruction;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
    }

    private void newInstruction(int? finished)
    {
        print("Mandando " +cont);
        if(cont <= actions.Length-1)
        {
            if (SendInstruction.sendInstruction != null && cont != -1)
            {
                SendInstruction.sendInstruction((int)actions[cont]);
                cont++;
            }
            else
            {
                print("no suscriptors");
            }
            
        }
    }
}


/*

    private void Awake()
    {
        SendInstruction.finishInstruction += newInstruction;
    }
    
    private void OnDestroy()
    {
        SendInstruction.finishInstruction -= newInstruction;
    }
    
    SendInstruction.sendInstruction((int)actions);

 */