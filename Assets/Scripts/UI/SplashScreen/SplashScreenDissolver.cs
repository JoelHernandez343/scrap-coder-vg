// Alberto García and Joel Harim Hernández Javier @ 2022
// Github: https://github.com/Ehad46
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace ScrapCoder.UI {
    public class SplashScreenDissolver : MonoBehaviour {
        // Internal types
        enum FadeState { FullDisplayed, Appearing, Fading, Disappeared }

        class ListenersContainer {
            List<System.Action>[] listenersArray =
                new List<System.Action>[
                    System.Enum.GetNames(typeof(SplashScreenEvent)).Length
                ];

            public List<System.Action> this[SplashScreenEvent eventType] {
                get => listenersArray[(int)eventType] ??= new List<System.Action>();
            }
        }

        // Editor variables
        [SerializeField] List<Image> UIImages;
        [SerializeField] List<TextMeshProUGUI> textMeshProUGUIs;

        [SerializeField] float fade;

        // State variables
        FadeState fadeState;

        ListenersContainer listenersContainer = new ListenersContainer();

        // Lazy variables
        List<Material> _materials;
        List<Material> materials {
            get {
                if (_materials != null) return _materials;

                _materials = new List<Material>();
                _materials.AddRange(UIImages.ConvertAll(i => i.material));
                _materials.AddRange(textMeshProUGUIs.ConvertAll(t => t.fontSharedMaterial));

                return _materials;
            }
        }

        Canvas _canvas;
        Canvas canvas => _canvas ??= (GetComponent<Canvas>() as Canvas);

        void Start() {
            fadeState = FadeState.FullDisplayed;
            fade = 1f;
            SetFade();
        }

        void Update() {

            if (Input.GetKeyDown(KeyCode.Space) && fadeState != FadeState.Disappeared) {
                fadeState = FadeState.Fading;
            }

            if (fadeState == FadeState.Fading) {
                fade -= Time.deltaTime * 0.75f;
                SetFade();
            } else if (fadeState == FadeState.Appearing) {
                fade += Time.deltaTime * 0.75f;
                SetFade();
            }
        }

        void SetFade() {
            fade = (fade > 1) ? 1 : (fade < 0) ? 0 : fade;

            materials.ForEach(m => m.SetFloat("_Fade", fade));

            if (fade == 0f) {
                fadeState = FadeState.Disappeared;
                OnTriggerEvent(eventType: SplashScreenEvent.OnDissapear);
            } else if (fade == 1f) {
                fadeState = FadeState.FullDisplayed;
                OnTriggerEvent(eventType: SplashScreenEvent.OnFullAppearance);
            }

            canvas.enabled = fade > 0f ? true : false;
        }

        public void AddListener(System.Action listener, SplashScreenEvent eventType = SplashScreenEvent.OnDissapear) {
            listenersContainer[eventType].Add(listener);
        }

        void OnTriggerEvent(SplashScreenEvent eventType) {
            listenersContainer[eventType].ForEach(listener => listener?.Invoke());
        }
    }
}