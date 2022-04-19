// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.Interpreter {
    public class Executer : MonoBehaviour {

        // Static variables
        public static Executer instance;

        // State variables
        Stack<IInterpreterElement> stack = new Stack<IInterpreterElement>();

        string nextLocalAnswer = null;
        bool executeNext;

        public bool stopped;

        // Lazy variables
        Analyzer _analyzer;
        Analyzer analyzer => _analyzer ??= (GetComponent<Analyzer>() as Analyzer);

        // Methods
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void FixedUpdate() {
            if (stopped) {
                ResetState();
            } else if (executeNext) {
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
            nextLocalAnswer = null;
            stack.Clear();
        }

        public void Execute() {
            stopped = false;
            ResetState();

            var (isValid, beginning) = analyzer.Analize();

            if (!isValid) return;

            PushNext(beginning.interpreterElement);
            ExecuteInNextFrame();
        }

        public void PushNext(IInterpreterElement e) {
            e.Reset();
            stack.Push(e);
        }

        void ExecuteNext(string answer = null) {
            executeNext = false;

            if (stack.Peek().Controller.type == VisualNodes.NodeType.End) {
                stack.Pop();

                Debug.Log("Execution is finished");
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
            executeNext = true;
        }

        void LocalAnswer() {
            var LocalAnswer = nextLocalAnswer;
            nextLocalAnswer = null;

            ExecuteNext(LocalAnswer);
        }

        void ReceiveAnswer(int _) {
            if (stopped) return;

            string answer = null; //RobotController.instance.answer;

            ExecuteNext(answer);
        }

    }
}