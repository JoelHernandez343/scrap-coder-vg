using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

using ScrapCoder.GameInput;
using ScrapCoder.UI;

public class InteractCopper : InteractScript {

    // Lazy variables
    Editor editor => InterfaceCanvas.instance.editorVisibiltyManager;

    // Methods
    void Start() {
        interactCollider = GetComponent<BoxCollider2D>();
        objectTransform = GetComponent<Transform>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (editor == null) return;

        if (interactable && InputController.instance.GetButtonDown("Interact", ignoreContainer: true) || !interactable && editor.isVisible) {
            Programar();
        }
    }

    void Programar() {
        editor.SetVisible(!editor.isVisible);
    }
}
