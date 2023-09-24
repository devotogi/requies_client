using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    private Queue<ArraySegment<byte>> _packetQueue = new Queue<ArraySegment<byte>>();
    private object _lock = new object();

    public void Push(ArraySegment<byte> packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    public ArraySegment<byte> Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count == 0)
                return null;

            else
                return _packetQueue.Dequeue();
        }
    }

    public int Count() { return _packetQueue.Count; }
}
