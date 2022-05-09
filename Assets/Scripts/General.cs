using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class General
{
    public static Action<int> evento_Energia;
    public static Action<int> fade;
}

public static class SendInstruction
{
    public static Action<int> sendInstruction;
    public static Action<int> finishInstruction;
}

public static class HideWallComs
{
    public static Action<int> hide;
}

public static class ResetPositionEvent
{
    public static Action<bool> reset;
    public static Action<bool> newPosition;
}
