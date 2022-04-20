// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScrapCoder.InputManagment;

namespace ScrapCoder.Interpreter {
    public class Executer : MonoBehaviour {

        // Internal types
        enum States { Running, Stopped, Stopping };

        // Static variables
        public static Executer instance;

        // State variables
        Stack<IInterpreterElement> stack = new Stack<IInterpreterElement>();

        string nextLocalAnswer = null;

        bool waitingForResponse = false;
        States state = States.Stopped;

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

            if (state == States.Running && !waitingForResponse) {
                var localAnswer = nextLocalAnswer;
                nextLocalAnswer = null;

                ExecuteNext(localAnswer);
            }

        }

        void Start() {
            // Suscribe to Robot controller
            SendInstruction.finishInstruction += ReceiveAnswer;
        }

        void ResetState() {
            state = States.Running;
            nextLocalAnswer = null;
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
            ExecuteInNextFrame();
        }

        public void Stop() {
            if (state == States.Stopped || state == States.Stopping) return;

            if (waitingForResponse) {
                state = States.Stopping;
                waitingForResponse = false;
            } else {
                state = States.Stopped;
            }
        }

        public void PushNext(IInterpreterElement e) {
            e.Reset();
            stack.Push(e);
        }

        void ExecuteNext(string answer = null) {
            waitingForResponse = true;

            if (stack.Peek().Controller.type == VisualNodes.NodeType.End) {
                stack.Pop();

                Debug.Log("Execution is finished");
                state = States.Stopped;
                return;
            }

            if (stack.Peek().IsFinished) {
                var finished = stack.Pop();
                var nextAnswer = "";

                if (finished.IsExpression) {
                    nextAnswer = answer;
                } else {
                    nextAnswer = null;

                    var next = finished.GetNextStatement();

                    if (next != null) {
                        PushNext(next);
                    }
                }

                ExecuteInNextFrame(nextAnswer);
                return;
            }

            stack.Peek().Execute(answer);
        }

        public void ExecuteInNextFrame(string answer = null) {
            nextLocalAnswer = answer;
            waitingForResponse = false;
        }

        void ReceiveAnswer(int _) {
            if (state == States.Stopping || state == States.Stopped) {
                state = States.Stopped;
            } else if (waitingForResponse) {
                string answer = null; //RobotController.instance.answer;
                ExecuteNext(answer);
            }
        }

    }
}