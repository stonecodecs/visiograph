using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private GameInput gameInput;
    private Vector3 playerVelocity;
    private float playerSpeed = 9f;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        gameInput = GameInput.Instance;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector2 movement = gameInput.GetMoveVectorNormalized();
        // Convert the 2D movement input to a 3D vector based on camera orientation
        Vector3 move = cameraTransform.forward * movement.y + cameraTransform.right * movement.x;
        controller.Move(move * Time.deltaTime * playerSpeed);
    }
}
