// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.GameInput;
using ScrapCoder.UI;

namespace ScrapCoder.Interpreter {
    public class Executer : MonoBehaviour {

        // Internal types
        enum States { Running, Stopped, Stopping };
        enum ExecutionState { Stopped, Immediately, NextFrame, WaitingForRobot }

        // Static variables
        public static Executer instance;

        // State variables
        Stack<InterpreterElement> stack = new Stack<InterpreterElement>();

        string nextArgument = null;

        States state = States.Stopped;
        ExecutionState executionState = ExecutionState.Stopped;

        float timer = 0f;
        float waitTime;

        ExecuterVelocity _velocity = ExecuterVelocity.Immediately;
        public ExecuterVelocity velocity {
            private set => _velocity = value;
            get => _velocity;
        }

        InterpreterElement currentElementWithFocus;

        // Lazy variables
        Analyzer _analyzer;
        Analyzer analyzer => _analyzer ??= (GetComponent<Analyzer>() as Analyzer);

        public string State => state.ToString();

        public bool isRunning => state == States.Running;

        // Methods
        void Awake() {
            UpdateWaitTime();

            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Update() {
            if (state != States.Running || executionState != ExecutionState.NextFrame) return;

            timer += Time.deltaTime;

            if (timer >= waitTime) {
                // Timer soft reset, min ~ timer -= waitTaime
                timer -= (int)System.Math.Floor(timer / waitTime) * waitTime;

                ExecuteNext();
            }
        }

        public void SetVelocity(ExecuterVelocity newVelocity) {
            if (velocity == newVelocity) return;

            var previousTiming = velocity;
            velocity = newVelocity;

            UpdateWaitTime();

            if (previousTiming < newVelocity) {
                timer = waitTime;
            }
        }

        void UpdateWaitTime() {
            if (velocity == ExecuterVelocity.Immediately) {
                waitTime = Time.fixedDeltaTime;
            } else if (velocity == ExecuterVelocity.EverySecond) {
                waitTime = 1f;
            } else if (velocity == ExecuterVelocity.EveryThreeSeconds) {
                waitTime = 3f;
            }
        }

        void Start() {
            // Suscribe to Robot controller
            SendInstruction.finishInstruction += ReceiveAnswer;
        }

        void ResetState() {
            state = States.Running;
            executionState = ExecutionState.Stopped;
            timer = 0f;

            nextArgument = null;
            stack.Clear();
        }

        public void Execute() {
            if (state == States.Running) {
                MessagesController.instance.AddMessage(
                    message: "El ejecutor ya se encuentra ejecutándose.",
                    type: MessageType.Warning
                );
                return;
            }
            if (state == States.Stopping) {
                MessagesController.instance.AddMessage(
                    message: "El ejecutor está esperando que el robot termine.",
                    type: MessageType.Warning
                );
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

            MessagesController.instance.AddMessage(
                message: "Ejecución terminada."
            );

            if (force) {
                state = States.Stopped;
            } else {
                state = executionState == ExecutionState.WaitingForRobot
                    ? States.Stopping
                    : States.Stopped;
            }

            executionState = ExecutionState.Stopped;

            ClearCurrentFocus();
        }

        public void PushNext(InterpreterElement next) {
            if (next == null) return;

            next.Reset();
            stack.Push(next);
        }

        void ExecuteNext() {
            executionState = ExecutionState.WaitingForRobot;

            var current = stack.Peek();

            if (current.Controller.type == NodeType.End) {
                stack.Pop();
                Stop(force: true);

                return;
            }

            if (current.IsFinished) {
                stack.Pop();

                if (current.IsExpression) {
                    ExecuteInmediately(argument: nextArgument);
                } else {
                    PushNext(next: current.GetNextStatement());
                    ExecuteInNextFrame();
                }
            } else {
                SetFocusOn(current);
                try {
                    current.Execute(argument: nextArgument);
                } catch (System.Exception e) {
                    Debug.LogException(e);
                    Stop(force: true);
                }
            }

            if (executionState == ExecutionState.Immediately) {
                if (velocity != ExecuterVelocity.Immediately) {
                    ExecuteInNextFrame(argument: nextArgument);
                } else {
                    ExecuteNext();
                }
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

        void SetFocusOn(InterpreterElement element) {
            if (currentElementWithFocus == element) return;

            currentElementWithFocus?.LoseFocus();

            if (velocity != ExecuterVelocity.Immediately) {
                currentElementWithFocus = element;
                currentElementWithFocus.GetFocus();
            }
        }

        void ClearCurrentFocus() {
            currentElementWithFocus?.LoseFocus();
            currentElementWithFocus = null;
        }

        void ReceiveAnswer(int? answer) {
            Debug.Log("Recibi respuesta " + answer);
            if (state == States.Stopping || state == States.Stopped) {
                state = States.Stopped;
            } else if (executionState == ExecutionState.WaitingForRobot) {
                nextArgument = answer != null ? $"{answer}" : null; //RobotController.instance.answer;
                ExecuteNext();
            }
        }

    }
}