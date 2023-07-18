using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{

    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    
    [SerializeField] private float clampAngle = 80f;  
    private GameInput gameInput;
    private Vector3 startingRotation;

    protected override void Awake() {
        gameInput = GameInput.Instance;
        base.Awake(); //superconstructor
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
       if (vcam.Follow) {
            if (stage == CinemachineCore.Stage.Aim) {
                if (startingRotation == null) {
                    startingRotation = transform.localRotation.eulerAngles;
                }
                Vector2 deltaInput = gameInput.GetMouseDelta();
                startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle); 
                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
                
            }
            
       }
    }
}
