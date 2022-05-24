// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;

namespace ScrapCoder.Interpreter {
    public class ConstantBooleanBuilder : InterpreterElementBuilder {

        // Internal types
        public enum TypeOfBoolean { True, False }

        // Editor variables
        [SerializeField] TypeOfBoolean typeOfBoolean;

        // Methods
        public override InterpreterElement GetInterpreterElement(List<InterpreterElement> parentList) {
            return new ConstantBooleanInterpreter(
                parentList: parentList,
                controllerReference: Controller,
                typeOfBoolean: typeOfBoolean
            );
        }

    }

    class ConstantBooleanInterpreter : InterpreterElement {

        // State variables
        ConstantBooleanBuilder.TypeOfBoolean typeOfBoolean;

        // Lazy variables
        public override bool isExpression => true;

        // Methods
        public override void Execute(string argument) {
            Executer.instance.ExecuteInmediately(
                argument: typeOfBoolean == ConstantBooleanBuilder.TypeOfBoolean.True
                    ? "true" : "false"
            );

            isFinished = true;
        }

        public override InterpreterElement NextStatement() => null;

        public ConstantBooleanInterpreter(
            List<InterpreterElement> parentList,
            NodeController controllerReference,
            ConstantBooleanBuilder.TypeOfBoolean typeOfBoolean
        ) : base(parentList, controllerReference) {

            this.typeOfBoolean = typeOfBoolean;

        }

    }
}
