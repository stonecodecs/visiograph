import struct
import socket
from sklearn.datasets import load_iris
import numpy as np

# Load the Iris dataset -- EXAMPLE
iris = load_iris()
X = iris.data
X = X[:, :3] # use first 3 dimensions (in real applications, use tSNE)
y = iris.target

# Define the batch size
batch_size = 50

# Establish a TCP connection
server_address = ("localhost", 12345)
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.bind(server_address)
sock.listen(1)
print('Waiting for Unity connection...')

# Accept a connection from Unity
connection, client_address = sock.accept()
print('Unity connected!')

try:
    # Send data in batches
    num_batches = int(np.ceil(len(X) / batch_size))
    print(num_batches)
    connection.sendall(struct.pack("i", len(X)))  # Send the number of expected rows
    
    # Send batch size (num of rows)
    connection.sendall(struct.pack("i", batch_size))

    for i in range(num_batches):
        start_idx = i * batch_size
        end_idx = (i + 1) * batch_size
        batch_X = X[start_idx:end_idx]
        batch_y = y[start_idx:end_idx]

        print(i)
        print(batch_X)
        print(batch_y)
        
        # Serialize and send the data
        # floats are 8 bytes in python
        # => need to consider this and use doubles in C#
        connection.sendall(batch_X.tobytes())
        connection.sendall(batch_y.tobytes())
        
        # Wait for Unity to process the batch
        response = connection.recv(1)
        if response != b'\x01': # reads response byte - line 82 in DataReceivier.cs
            print("broken")
            break
        else:
            print("received confirmation")
        print('end')

finally:
    # Clean up the connection
    connection.close()
    sock.close()