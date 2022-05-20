using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Dissolve : MonoBehaviour {
    // Internal types
    enum FadeState { FullDisplayed, Appearing, Fading, Disappeared }

    // Editor variables
    [SerializeField] List<Image> UIImages;
    [SerializeField] List<TextMeshProUGUI> textMeshProUGUIs;

    [SerializeField] float fade;

    // State variables
    FadeState fadeState;

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

        if (Input.GetKeyDown(KeyCode.Space)) {
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
        } else if (fade == 1f) {
            fadeState = FadeState.FullDisplayed;
        }

        canvas.enabled = fade > 0f ? true : false;
    }
}
