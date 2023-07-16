using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    private List<GameObject> datapoints;
    private List<double> X;
    private List<int> y;

    private void Awake () {
        datapoints = new List<GameObject>();
    }

    public void inputData(List<double> X, List<int> y) {
        this.X = X;
        this.y = y;
    }
    
    public void CreateGameObjectDatapoints() {
        // THIS HANGS BECAUSE CONCURRENCY ISSUE: Unity objects/operations should be done on main thread
        // Task.Run is on its separate thread
        for(int i = 0; i < X.Count; i+=Constants.NUM_FEATURES) { 
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = new Vector3((float)X[i] * Constants.GLOBAL_SCALE, 
                                                    (float)X[i+1] * Constants.GLOBAL_SCALE, 
                                                    (float)X[i+2] * Constants.GLOBAL_SCALE); // required field
            sphere.transform.localScale = Vector3.one;
            sphere.transform.eulerAngles = Vector3.zero; // rotation, but irrelevant for spheres
            datapoints.Add(sphere);
            Debug.Log("Number of datapoints: " + datapoints.Count);
        }
    }

    public void ClearGameObjectDatapoints() { 
        foreach (GameObject d in datapoints) { 
            Destroy(d);
        }
        datapoints.Clear();
        Debug.Log("cleared, len now " + datapoints.Count);
    }

}
