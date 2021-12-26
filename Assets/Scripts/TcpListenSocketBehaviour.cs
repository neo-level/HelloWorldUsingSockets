using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// Simulated server side.
/// </summary>
public class TcpListenSocketBehaviour : MonoBehaviour
{
    private TcpListener _listener;

    [HideInInspector] public bool isReady;

    [Tooltip("The port the service is running on.")]
    public int port = 9021;

    private void Start()
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        _listener.BeginAcceptSocket(Socket_Connected, _listener);
        isReady = true;
    }

    private void OnDestroy()
    {
        _listener?.Stop();
        _listener = null;
    }

    private void Socket_Connected(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            var socket = (result.AsyncState as TcpListener)?.EndAcceptSocket(result);
            int offset = 0;
            var state = new StateObject(socket);
            socket?.BeginReceive(state.Buffer, offset, state.Buffer.Length, SocketFlags.None, Socket_Received, state);
        }
    }

    private void Socket_Received(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            var state = result.AsyncState as StateObject;
            var bytesIn = state.Socket.EndReceive(result);
            if (bytesIn > 0)
            {
                int index = 0;
                var message = Encoding.ASCII.GetString(state.Buffer, index, bytesIn);
                print($"from client: {message}");
            }

            int offset = 0;
            var newState = new StateObject(state?.Socket);
            state?.Socket.BeginReceive(state.Buffer, offset, state.Buffer.Length, SocketFlags.None, Socket_Received,
                newState);
        }
    }
}