using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TCPClient : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // Replace with the server's IP address.
    public int serverPort = 12345; // Specify the port the server is listening on.

    private TcpClient tcpClient;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    private void Start()
    {
        try
        {
            // Create a TCP client and connect to the server with the provided IP and port.
            tcpClient = new TcpClient();
            tcpClient.Connect(serverIP, serverPort);
            stream = tcpClient.GetStream();

            Debug.Log("TCP client connected to " + serverIP + ":" + serverPort);

            // Start listening for server responses in a separate thread.
            StartReceiving();
        }
        catch (SocketException e)
        {
            Debug.LogError("Error setting up TCP client: " + e.Message);
        }
    }

    private void StartReceiving()
    {
        try
        {
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting to receive data: " + e.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        int bytesRead = stream.EndRead(ar);

        if (bytesRead > 0)
        {
            string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
            HandleResponse(receivedData);

            // Continue listening for more data.
            StartReceiving();
        }
        else
        {
            // Connection closed or lost. Handle it here.
            Debug.Log("Server connection closed.");
            tcpClient.Close();
        }
    }

    private void HandleResponse(string responseMessage)
    {
        // Handle the received response from the server.
        Debug.Log("Received response from server: " + responseMessage);
    }

    // Method to send a message to the server.
    public void SendMessageToServer(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    private void OnDestroy()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }
}