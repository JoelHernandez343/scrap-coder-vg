// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.InputManagment;

namespace ScrapCoder.Interpreter {
    public class Executer : MonoBehaviour {

        // Internal types
        enum States { Running, Stopped, Stopping };
        enum ExecutionState { Stopped, Immediately, NextFrame, WaitingForRobot }

        // Static variables
        public static Executer instance;

        // State variables
        Stack<IInterpreterElement> stack = new Stack<IInterpreterElement>();

        string nextArgument = null;

        States state = States.Stopped;
        ExecutionState executionState = ExecutionState.Stopped;

        // Lazy variables
        Analyzer _analyzer;
        Analyzer analyzer => _analyzer ??= (GetComponent<Analyzer>() as Analyzer);

        public string State => state.ToString();

        public bool isRunning => state == States.Running;

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void FixedUpdate() {
            if (state == States.Running && executionState == ExecutionState.NextFrame) {
                ExecuteNext();
            }
        }

        void Start() {
            // Suscribe to Robot controller
            SendInstruction.finishInstruction += ReceiveAnswer;
        }

        void ResetState() {
            state = States.Running;
            executionState = ExecutionState.Stopped;

            nextArgument = null;
            stack.Clear();
        }

        public void Execute() {
            if (state == States.Running) {
                Debug.LogWarning("Already executing!");
                return;
            }
            if (state == States.Stopping) {
                Debug.LogWarning("Waiting for termination.");
                return;
            }

            InputController.instance.ClearFocus();

            var (isValid, beginning) = analyzer.Analize();
            if (!isValid) return;

            ResetState();

            PushNext(beginning.interpreterElement);
            ExecuteNext();
        }

        public void Stop(bool force = false) {
            if (state == States.Stopped || state == States.Stopping) return;

            Debug.Log("Execution is finished");

            if (force) {
                state = States.Stopped;
            } else {
                state = executionState == ExecutionState.WaitingForRobot
                    ? States.Stopping
                    : States.Stopped;
            }

            executionState = ExecutionState.Stopped;
        }

        public void PushNext(IInterpreterElement next) {
            if (next == null) return;

            next.Reset();
            stack.Push(next);
        }

        void ExecuteNext() {
            executionState = ExecutionState.WaitingForRobot;

            if (stack.Peek().Controller.type == NodeType.End) {
                stack.Pop();

                Stop(force: true);

                return;
            }

            if (stack.Peek().IsFinished) {
                var finished = stack.Pop();

                if (finished.IsExpression) {
                    ExecuteInmediately(argument: nextArgument);
                } else {
                    PushNext(next: finished.GetNextStatement());
                    ExecuteInNextFrame();
                }
            } else {
                stack.Peek().Execute(argument: nextArgument);
            }

            if (executionState == ExecutionState.Immediately) {
                ExecuteNext();
            }
        }

        public void ExecuteInNextFrame(string argument = null) {
            nextArgument = argument;
            executionState = ExecutionState.NextFrame;
        }

        public void ExecuteInmediately(string argument = null) {
            nextArgument = argument;
            executionState = ExecutionState.Immediately;
        }

        void ReceiveAnswer(int _) {
            if (state == States.Stopping || state == States.Stopped) {
                state = States.Stopped;
            } else if (executionState == ExecutionState.WaitingForRobot) {
                nextArgument = null; //RobotController.instance.answer;
                ExecuteNext();
            }
        }

    }
}