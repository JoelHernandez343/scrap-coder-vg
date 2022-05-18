// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public enum NodeType {
        None = -1,
        Begin = 0,
        End = 1,
        Repeat = 2,
        Conditional = 3,
        ElseConditional = 4,
        ConditionAnd = 5,
        ConditionOr = 7,
        ConditionNegation = 8,
        Walk = 9,
        Interact = 12,
        AddToVariable = 14,
        DecreaseVariable = 15,
        RepeatNTimes = 18,
        Rotate = 19,
        TrueConstant = 20,
        FalseConstant = 21,
        Variable = 22,
        NumericValue = 23,
        SetVariable = 24,
        NumericOperation = 35,
        NumericValueComparison = 36,
        Array = 37,
        AddToArray = 38,
        ClearArray = 39,
        RemoveFromArray = 40,
        ValueOfArray = 41,
        LengthOfArray = 42,
        SetValueOfArray = 43,
        InsertInArray = 44,
        ScanInstruction = 45,
        ScanAndAddToArray = 46,
        ScanAndSetValueOfArray = 47,
    }

}
