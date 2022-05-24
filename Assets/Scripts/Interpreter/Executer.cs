// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.VisualNodes;
using ScrapCoder.GameInput;
using ScrapCoder.UI;
using ScrapCoder.Tutorial;

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

        float timerForVelocity = 0f;
        float waitTimeForVelocity;

        float timerForFlickering = 0f;
        const float waitTimeForFlickering = 0.25f;

        ExecuterVelocity _velocity = ExecuterVelocity.Immediately;
        public ExecuterVelocity velocity {
            private set => _velocity = value;
            get => _velocity;
        }

        InterpreterElement currentElementWithFocus;
        InterpreterElement currentCode;

        // Lazy variables
        Analyzer _analyzer;
        Analyzer analyzer => _analyzer ??= (GetComponent<Analyzer>() as Analyzer);

        public string State => state.ToString();

        public bool isRunning => state == States.Running;

        // Methods
        void Awake() {
            UpdateWaitTime();

            if (instance != null) {
                Debug.Log("Wait, is there another Executor?!");
                Destroy(this);
                return;
            }

            instance = this;
        }

        void Update() {
            if (state != States.Running) return;

            if (executionState == ExecutionState.NextFrame) {
                timerForVelocity += Time.deltaTime;

                if (timerForVelocity >= waitTimeForVelocity) {
                    // Timer soft reset, min ~ timer -= waitTaime
                    timerForVelocity -= (int)System.Math.Floor(timerForVelocity / waitTimeForVelocity) * waitTimeForVelocity;
                    ExecuteNext();
                }
            }

            if (currentElementWithFocus != null && velocity != ExecuterVelocity.Immediately) {
                timerForFlickering += Time.deltaTime;

                if (timerForFlickering >= waitTimeForFlickering) {
                    timerForFlickering -= waitTimeForFlickering;
                    currentElementWithFocus.ChangeFlickeringState();
                }

                currentElementWithFocus.SetVisualState();
            }
        }

        public void SetVelocity(ExecuterVelocity newVelocity) {
            if (velocity == newVelocity) return;

            var previousTiming = velocity;
            velocity = newVelocity;

            UpdateWaitTime();

            if (previousTiming < newVelocity) {
                timerForVelocity = waitTimeForVelocity;
            }
        }

        void UpdateWaitTime() {
            if (velocity == ExecuterVelocity.Immediately) {
                waitTimeForVelocity = Time.fixedDeltaTime;
            } else if (velocity == ExecuterVelocity.EverySecond) {
                waitTimeForVelocity = 1f;
            } else if (velocity == ExecuterVelocity.EveryThreeSeconds) {
                waitTimeForVelocity = 3f;
            }
        }

        void Start() {
            // Suscribe to Robot controller
            SendInstruction.finishInstruction += ReceiveAnswer;
        }

        void ResetState() {
            state = States.Running;
            executionState = ExecutionState.Stopped;
            timerForVelocity = 0f;

            nextArgument = null;

            currentCode = null;

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

            TutorialController.instance.ReceiveSignal(signal: "executerCalled");

            ResetState();

            currentCode = beginning.GetInterpreterElement();
            // TreePresentation(currentCode);

            PushNext(currentCode);
            ExecuteNext();
        }

        public void Stop(bool successfully = true) {
            if (state == States.Stopped || state == States.Stopping) return;



            if (successfully) {
                MessagesController.instance.AddMessage(
                    message: "Ejecución terminada."
                );
                TutorialController.instance.ReceiveSignal(signal: "executerSuccessfullyFinished");
            } else {
                MessagesController.instance.AddMessage(
                    message: "Ejecución terminada con errores.",
                    type: MessageType.Error
                );
                TutorialController.instance.ReceiveSignal(signal: "executerFinishedWithError");
            }

            // Here we must indicate to the robot that execution is finished

            state = States.Stopped;
            executionState = ExecutionState.Stopped;

            ClearCurrentFocus();
        }

        public void PushNext(InterpreterElement next) {
            if (next == null) return;

            next.ResetState();
            stack.Push(next);
        }

        void ExecuteNext() {
            executionState = ExecutionState.WaitingForRobot;

            Debug.Assert(stack.Count > 0, "Wtf Unity");
            var current = stack.Peek();

            if (current.type == NodeType.End) {
                stack.Pop();
                Stop();

                return;
            }

            if (current.isFinished) {
                stack.Pop();

                if (current.isExpression) {
                    ExecuteInmediately(argument: nextArgument);
                } else {
                    PushNext(next: current.NextStatement());
                    ExecuteInNextFrame();
                }
            } else {
                SetFocusOn(current);
                try {
                    current.Execute(argument: nextArgument);
                } catch (System.Exception e) {
                    Debug.LogException(e);
                    Stop(successfully: false);
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

            timerForFlickering = 0f;

            if (velocity != ExecuterVelocity.Immediately) {
                currentElementWithFocus = element;
                currentElementWithFocus.GetFocus();
            }
        }

        void ClearCurrentFocus() {
            timerForFlickering = 0f;
            currentElementWithFocus?.LoseFocus();
            currentElementWithFocus = null;
        }

        void ReceiveAnswer(int? answer) {
            bool received = false;

            if (state == States.Stopping || state == States.Stopped) {
                state = States.Stopped;
            } else if (executionState == ExecutionState.WaitingForRobot) {
                Debug.Log($"[Executer] Answer processed: {(answer == null ? "void" : $"{answer}")}");
                received = true;
                nextArgument = answer != null ? $"{answer}" : null; //RobotController.instance.answer;
            }

            if (!received) {
                Debug.Log($"[Executer] Answer ignored: {(answer == null ? "void" : $"{answer}")}");
            } else {
                ExecuteNext();
            }
        }

        void TreePresentation(InterpreterElement element, string parent = "") {

            Debug.Log($"{parent}{{ referenceName: {element.controllerReference?.name}, type: {element.type} }}");

            element.listOfChildren.ForEach(children =>
                children.ForEach(child =>
                    TreePresentation(child, $"{parent}{element.controllerReference?.name}->")
                )
            );

        }

    }
}