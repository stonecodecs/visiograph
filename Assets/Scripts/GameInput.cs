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
        playerInputActions = new PlayerInputActions();  
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
        Debug.Log("Move vector activated," + playerInputActions.Player.Move.ReadValue<Vector2>());
        return playerInputActions.Player.Move.ReadValue<Vector2>();
        // Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        // // // getKey is used for press and hold (WASD movement), getKeyDown is once (space jump)
        // // // legacy code Input.GetKey
        // // if (Input.GetKey(KeyCode.W)) {
        // //     inputVector.y = +1;
        // // }
        // // if (Input.GetKey(KeyCode.A)) {
        // //     inputVector.x = -1;
        // // }
        // // if (Input.GetKey(KeyCode.S)) {
        // //     inputVector.y = -1;
        // // }
        // // if (Input.GetKey(KeyCode.D)) {
        // //     inputVector.x = +1;
        // // }
        // Debug.Log(inputVector);
        // return inputVector;

    }
}
