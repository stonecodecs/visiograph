using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private static GameInput _instance;
    public static GameInput Instance {
        get {
            return _instance;
        }
    }

    private PlayerInputActions playerInputActions;
    private void Awake() {
        // if no other instance
        if (_instance != null && _instance != this) {
            Debug.Log("instance existed, destroyed");
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        playerInputActions = new PlayerInputActions(); // WASD, Mouse Cursor
        // TODO: Add HMD, VR controller
        Cursor.visible = false;
    }

    private void OnEnable() {
        playerInputActions.Player.Enable();
    }

    private void OnDisable() {
        playerInputActions.Player.Disable();
    }

    public Vector2 GetMouseDelta() {
        return playerInputActions.Player.Look.ReadValue<Vector2>();
    }

    public Vector2 GetMoveVectorNormalized() {
        return playerInputActions.Player.Move.ReadValue<Vector2>();
    }

    public bool GetMouseLMB() {
        float lmb = playerInputActions.Player.Select.ReadValue<float>(); // 0.0f or 1.0f
        Debug.Log("lmb value: " + lmb.ToString());
        if (lmb > 0.5f) { return true; }
        else { return false; }
    }
}
