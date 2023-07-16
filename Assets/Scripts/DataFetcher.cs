using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

public static class Constants {
    public static int NUM_FEATURES = 3;
    public static int GLOBAL_SCALE = 10; // space out spheres by 10 
}

public class DataFetcher : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer;
    private int batchIndex;
    private const int PORT = 12345;
    private int numRows = 0;
    private int batchSize = 100;
    private List<double> X = new List<double>();
    private List<int> y = new List<int>();
    private DataProcessor dataProcessor;
    private TaskCompletionSource<bool> dataProcessingTaskCompletionSource;


    // Start is called before the first frame update
    void Start()
    {
        dataProcessor = GetComponent<DataProcessor>();
        if (dataProcessor == null) {
            Debug.LogError("No DataProcessor script found on GameObject");
            return;
        }

        dataProcessingTaskCompletionSource = new TaskCompletionSource<bool>();
        client = new TcpClient("localhost", PORT);
        stream = client.GetStream();
        
        // Receive the number of batches
        byte[] numRowsBuffer = new byte[4];
        stream.Read(numRowsBuffer, 0, numRowsBuffer.Length);
        numRows = BitConverter.ToInt32(numRowsBuffer, 0);
        Debug.Log("numRows: " + numRows);

        stream.Read(numRowsBuffer, 0, numRowsBuffer.Length); // reusing numRowsBuffer for batchsize
        batchSize = BitConverter.ToInt32(numRowsBuffer, 0);
        Debug.Log("batchsize: " + batchSize);

        int numBatches = ((numRows - 1) / batchSize) + 1;
        Debug.Log("numBatches: " + numBatches);
        
        // Process each batch asynchronously
        Task.Run(async () =>
        {
            Debug.Log("Running async Task");
            for (int i = 0; i < numBatches; i++)
            {
                Debug.Log("iter" + i);
                if(i == numBatches - 1){
                    if (numRows % batchSize != 0){
                        batchSize = (numRows % batchSize);
                    }
                    Debug.Log("final batchsize = " + batchSize);
                }
                await ProcessBatch();
            }

            // Close the connection when all batches are processed
            stream.Close();
            client.Close();
            Debug.Log("Data transfer completed!");
            dataProcessingTaskCompletionSource.SetResult(true);
        });

        Debug.Log("after task run");
    }

    void Update(){
        if (dataProcessingTaskCompletionSource != null && dataProcessingTaskCompletionSource.Task.IsCompleted) {
            dataProcessor.inputData(X, y);
            dataProcessor.CreateGameObjectDatapoints(); // create objects
            dataProcessingTaskCompletionSource = null; // no need to check threads anymore...?
        }
    }

    private async Task ProcessBatch()
    {
        Debug.Log("ProcessBatch called, batchsize: " + batchSize);

        // Receive and process a batch
        byte[] batchXBuffer = new byte[batchSize*Constants.NUM_FEATURES*sizeof(double)]; // Assuming batch size of 10 and 4 features per sample (float32)
        byte[] batchYBuffer = new byte[batchSize*sizeof(int)]; // Assuming batch size of 10 and 1 target value per sample (int32)
        
        await stream.ReadAsync(batchXBuffer, 0, batchXBuffer.Length);
        await stream.ReadAsync(batchYBuffer, 0, batchYBuffer.Length);

        // Deserialize the data
        double[] batchX = new double[batchSize*Constants.NUM_FEATURES];
        int[] batchY = new int[batchSize]; // assumes non-multilabel classification
        

        for (int i = 0; i < batchXBuffer.Length; i += sizeof(double))
        {
            batchX[i / sizeof(double)] = BitConverter.ToDouble(batchXBuffer, i);
        }

        for (int i = 0; i < batchYBuffer.Length; i += sizeof(int))
        {
            batchY[i / sizeof(int)] = BitConverter.ToInt32(batchYBuffer, i);
        }

        // create objects
        for(int i = 0; i < batchSize*Constants.NUM_FEATURES; i+=Constants.NUM_FEATURES) { // length should ALWAYS be multiple of NUM_FEATURES
// save color, shape values in future?
            X.Add(batchX[i]);
            X.Add(batchX[i+1]);
            X.Add(batchX[i+2]);
            y.Add(batchY[i/Constants.NUM_FEATURES]);
        }

        Debug.Log("X: " + string.Join(", ", X));
        Debug.Log("Y: " + string.Join(", ", y));

        // Send a response to Python indicating successful processing
        byte[] responseBuffer = new byte[1] { 1 };
        await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
        Debug.Log("response sent");
    }
}
