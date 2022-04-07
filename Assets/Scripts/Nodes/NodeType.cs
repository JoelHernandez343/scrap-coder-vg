// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.VisualNodes {

    public enum NodeType {
        Begin = 0,
        End = 1,
        Repeat = 2,
        Conditional = 3,
        ElseConditional = 4,
        ConditionAnd = 5,
        Generic = 6,
        ConditionOr = 7,
        ConditionNegation = 8,
        Caminar_hacia_adelante = 9,
        Girar_hacia_la_izquierda = 10,
        Girar_hacia_la_derecha = 11,
        Interactuar = 12,
        Establecer_variable = 13,
        Aumentar_variable_en_1 = 14,
        Decrecer_variable_en_1 = 15,
        Condition_variable_es_igual_a = 16,
        Condition_detectar = 17,
        RepeatNTimes = 18,
    }

}
