using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {
    [SerializeField] private Texture2D cursorTexture;

    private void Awake() {
        Debug.Log("CursorManager radi");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.SetCursor(
            cursorTexture,
            Vector2.zero,
            CursorMode.ForceSoftware
        );
    }
}