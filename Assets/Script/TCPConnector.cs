using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TCPConnector
{
    private Socket _socket;
    private IPEndPoint _ipEndPoint;
    public Socket ConnectSocket { get { return _socket; } }

    public TCPConnector()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public bool ConnectTo(string ip, int port)
    {
        try
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket.Connect(_ipEndPoint);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
        return true;
    }

}
