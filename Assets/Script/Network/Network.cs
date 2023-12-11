using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Network : MonoBehaviour
{
    private TCPConnector _filedConnector = new TCPConnector();
    private Thread _filedThread;

    private TCPConnector _loginConnector = new TCPConnector();
    private Thread _loginThread;


    private PacketHandler _packetHandler = new PacketHandler();

    private const int _recvBufferSize = 4096 * 5;
    private byte[] _recvBuffer = new byte[_recvBufferSize];
    public string LocalIp
    {
        get
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }

    void Awake()
    {
        Init();
    }

    void Start()
    {

    }

    void LoginServerConnect() 
    {
        if (_loginConnector.ConnectTo(Type.IP, Type.LoginPort))
        {
            _loginThread = new Thread(new ThreadStart(Login_TCPRecvProc));
            _loginThread.Start();
        }
    }

    void FiledServerConnect() 
    {
        if (_filedConnector.ConnectTo(Type.IP, Type.FieldPORT))
        {
            _filedThread = new Thread(new ThreadStart(Filed_TCPRecvProc));
            _filedThread.Start();
        }
    }

    void Init()
    {
        LoginServerConnect();
    }

    public void SendPacket(byte[] buffer, int sendSize, Type.ServerType serverType)
    {
        switch (serverType)
        {
            case Type.ServerType.Login:
                _loginConnector.ConnectSocket.Send(buffer, sendSize, SocketFlags.None);
                break;

            case Type.ServerType.Field:
                _filedConnector.ConnectSocket.Send(buffer, sendSize, SocketFlags.None);
                break;
        }
    }

    void Update()
    {
        while (true)
        {
            ArraySegment<byte> packet = PacketQueue.Instance.Pop();

            if (packet == null)
                break;

            if (packet != null)
            {
                _packetHandler.Handler(packet);
            }
        }
    }

    private void Login_TCPRecvProc()
    {
        int recvSize = 0;
        int readPos = 0;
        int writePos = 0;
        try
        {
            while (true)
            {
                recvSize = _loginConnector.ConnectSocket.Receive(_recvBuffer, writePos, _recvBuffer.Length - writePos, SocketFlags.None);

                if (recvSize < 1)
                {
                    _loginConnector.ConnectSocket.Close();
                    break;
                }

                writePos += recvSize;
                // [200][100][200][100]
                while (true)
                {
                    int dataSize = Math.Abs(writePos - readPos);

                    if (dataSize < 4) break;

                    ArraySegment<byte> pktCodeByte = new ArraySegment<byte>(_recvBuffer, readPos, readPos + sizeof(UInt16));
                    ArraySegment<byte> pktSizeByte = new ArraySegment<byte>(_recvBuffer, readPos + sizeof(UInt16), readPos + sizeof(UInt16));

                    Int16 pktCode = BitConverter.ToInt16(pktCodeByte);
                    Int16 pktSize = BitConverter.ToInt16(pktSizeByte);

                    if (pktSize > dataSize)
                        break;

                    ArraySegment<byte> segment = new ArraySegment<byte>(_recvBuffer, readPos, pktSize);
                    byte[] data = new byte[pktSize];

                    Array.Copy(segment.ToArray(), data, pktSize);

                    PacketQueue.Instance.Push(data);

                    // TODO 单捞磐 贸府
                    readPos += pktSize;

                    if (readPos == writePos)
                    {
                        readPos = 0;
                        writePos = 0;
                    }
                    else if (writePos >= 4096 * 4)
                    {
                        Buffer.BlockCopy(_recvBuffer, readPos, _recvBuffer, 0, dataSize);
                        writePos = dataSize;
                    }

                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Filed_TCPRecvProc()
    {
        int recvSize = 0;
        int readPos = 0;
        int writePos = 0;
        try
        {
            while (true)
            {
                recvSize = _filedConnector.ConnectSocket.Receive(_recvBuffer, writePos, _recvBuffer.Length - writePos, SocketFlags.None);

                if (recvSize < 1)
                {
                    _filedConnector.ConnectSocket.Close();
                    break;
                }

                writePos += recvSize;
                // [200][100][200][100]
                while (true)
                {
                    int dataSize = Math.Abs(writePos - readPos);

                    if (dataSize < 4) break;

                    ArraySegment<byte> pktCodeByte = new ArraySegment<byte>(_recvBuffer, readPos, readPos + sizeof(UInt16));
                    ArraySegment<byte> pktSizeByte = new ArraySegment<byte>(_recvBuffer, readPos + sizeof(UInt16), readPos + sizeof(UInt16));

                    Int16 pktCode = BitConverter.ToInt16(pktCodeByte);
                    Int16 pktSize = BitConverter.ToInt16(pktSizeByte);

                    if (pktSize > dataSize)
                        break;

                    ArraySegment<byte> segment = new ArraySegment<byte>(_recvBuffer, readPos, pktSize);
                    byte[] data = new byte[pktSize];

                    Array.Copy(segment.ToArray(), data, pktSize);

                    PacketQueue.Instance.Push(data);

                    // TODO 单捞磐 贸府
                    readPos += pktSize;

                    if (readPos == writePos)
                    {
                        readPos = 0;
                        writePos = 0;
                    }
                    else if (writePos >= 4096 * 4)
                    {
                        Buffer.BlockCopy(_recvBuffer, readPos, _recvBuffer, 0, dataSize);
                        writePos = dataSize;
                    }

                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
