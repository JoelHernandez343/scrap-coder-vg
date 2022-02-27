using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotInstructions : MonoBehaviour
{
    private enum Action { Walk, RotateLeft, RotateRight, Interact, None }
    [SerializeField] private Action[] actions;
    [SerializeField] private int cont = 0;
    void Start()
    {
        SendInstruction.finishInstruction += newInstruction;
        newInstruction(0);
    }

    private void OnDestroy()
    {
        SendInstruction.finishInstruction += newInstruction;
    }

    private void newInstruction(int finished)
    {
        print("Mandando " +cont);
        if(cont <= actions.Length-1)
        {
            SendInstruction.sendInstruction((int)actions[cont]);
            cont++;
        }
    }
}
