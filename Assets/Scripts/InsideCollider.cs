using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideCollider : MonoBehaviour
{
    public GameObject internalCollider; // Reference to the smaller internal solid collider

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + " entered");
        // Check if the entering collider is the Player collider
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.tag + " entered, disable");
            // Deactivate the internal collider
            internalCollider.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag + " exited");
        // Check if the exiting collider is the Player collider
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.tag + " enable");
            // Activate the internal collider
            internalCollider.SetActive(true);
        }
    }
}
