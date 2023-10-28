using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

public class TCPServer : MonoBehaviour
{
    public int port = 12345; // Specify the port to listen on.

    private TcpListener tcpListener;
    private Thread listenerThread;

    private void Start()
    {
        try
        {
            // Create a TCP listener on the specified port.
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Debug.Log("TCP server is listening on port " + port);

            // Start a separate thread to accept client connections.
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("Error setting up TCP server: " + e.Message);
        }
    }

    private void ListenForClients()
    {
        while (true)
        {
            try
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Debug.Log("Client connected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address);

                // Start a new thread to handle the client.
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
            catch (SocketException)
            {
                // Handle socket exceptions if necessary.
            }
        }
    }

    private void HandleClient(object clientObject)
    {
        TcpClient client = (TcpClient)clientObject;

        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    HandleMessage(data, client);

                    // Optionally, you can send a response to the client here.
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error handling client: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }

    private void HandleMessage(string message, TcpClient client)
    {
        // Handle the received message from the client.
        Debug.Log("Received message from client " + ((IPEndPoint)client.Client.RemoteEndPoint).Address + ": " + message);
    }

    private void OnDestroy()
    {
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }
    }
}