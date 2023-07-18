using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private GameInput gameInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 9f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        gameInput = GameInput.Instance;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // from 2D movement script,
        // TODO: Adapt to 3D floating space
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = gameInput.GetMoveVectorNormalized();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
