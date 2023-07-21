using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SelectionRay : MonoBehaviour
{
    public float raycastDistance = Mathf.Infinity; // Maximum distance for the raycast

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found in the scene. Please add a camera tagged as 'MainCamera'.");
            enabled = false;
        }
    }

    private void Update()
    {
        // Get the center of the camera viewport
        Vector3 cameraCenter = new Vector3(0.5f, 0.5f, 0f);
        
        // Cast a ray from the center of the camera viewport
        Ray ray = mainCamera.ViewportPointToRay(cameraCenter);

        // check raycast hits
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
            // TODO: On hit, give option to select it and display information
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
        }
        else
        {
            // for visualization, possibly delete later
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.white);
        }
    }
}