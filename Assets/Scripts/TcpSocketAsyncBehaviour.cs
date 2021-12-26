using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Collections;

/// <summary>
/// Simulated client side.
/// </summary>
[RequireComponent(typeof(TcpListenSocketBehaviour))]
public class TcpSocketAsyncBehaviour : MonoBehaviour
{
    private Socket _socket;

    [Tooltip("The port the service is running on.")]
    public int port = 9021;

    IEnumerator Start()
    {
        var listener = GetComponent<TcpListenSocketBehaviour>();

        while (!listener.isReady)
        {
            yield return null;
        }

        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(IPAddress.Parse("127.0.0.1"), port);
        
        int offset = 0;
        var message = Encoding.ASCII.GetBytes("hello, from Client!");
        _socket.BeginSend(message, offset, message.Length, SocketFlags.None, Send_Complete, _socket);
        
    }

    private void Send_Complete(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            var socket = result.AsyncState as Socket;
            var bytesSent = socket.EndSend(result);
            print($"{bytesSent} bytes sent");
        }
    }
}
