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
}
