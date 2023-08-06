using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float teleportDuration = .5f; // Time to complete teleportation
    [SerializeField] private float teleportOffset= 5f;
    public GameObject guiCircle;
    public float raycastDistance = Mathf.Infinity; // Maximum distance for the raycast
    private Camera mainCamera;
    private CharacterController controller;
    private GameInput gameInput;
    private Vector3 playerVelocity;
    private float playerSpeed = 9f;
    private Transform cameraTransform;

    private bool isTeleporting = false; // Flag to check if currently teleporting
    private Vector3 teleportTarget; // Teleport Target position
    

    private void Start()
    {
        mainCamera = Camera.main;
        guiCircle = GameObject.Find("GUICircle");
        guiCircle.SetActive(false);

        if (guiCircle == null)
        {
            Debug.LogError("GUI circle not found in the scene. Make sure the GUICircle GameObject has the GUICircle script attached.");
            enabled = false;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found in the scene. Please add a camera tagged as 'MainCamera'.");
            enabled = false;
        }
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
    
        // Get the center of the camera viewport
        Vector3 cameraCenter = new Vector3(0.5f, 0.5f, 0f);
        
        // Cast a ray from the center of the camera viewport
        Ray ray = mainCamera.ViewportPointToRay(cameraCenter);

        // check raycast hits
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

            Debug.Log("Raycast hit: " + hit.collider.gameObject.tag);
            guiCircle.SetActive(true); 
            guiCircle.transform.position = hit.point; // reveal GUIcircle on the point of contact

            //TODO: When selected (click on PC), should teleport slightly behind said location
            if (hit.collider.CompareTag("visual") && gameInput.GetMouseLMB())
                {
                    // OLD: instant teleport code
                    // Vector3 teleportPosition = hit.point - ray.direction.normalized * teleportOffset;
                    // transform.position = teleportPosition;
                    StartCoroutine(TeleportSmoothly(hit.point));
                }
        }
        else
        {
            // for visualization, possibly delete later
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.white);
            guiCircle.SetActive(false);
        }
    }

    // Coroutine for smooth teleportation
    private IEnumerator TeleportSmoothly(Vector3 targetPosition)
    {
        isTeleporting = true;
        teleportTarget = targetPosition;

        Vector3 startPosition = transform.position;
        float startTime = Time.time;

        while (Time.time < startTime + teleportDuration)
        {
            float t = (Time.time - startTime) / teleportDuration;
            transform.position = Vector3.Lerp(startPosition, teleportTarget, t);
            yield return null;
        }

        // Ensure the final position is exact
        transform.position = teleportTarget;

        isTeleporting = false;
    }
}
