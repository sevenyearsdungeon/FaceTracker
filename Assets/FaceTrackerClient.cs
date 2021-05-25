using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class FaceTrackerClient : MonoBehaviour
{
    public static Action<FrameData> OnCenterFaceUpdated;
    byte[] message = new byte[] { 1, 2, 3, 4 };
    bool run=false;
    // Use this for initialization
    async Task Start()
    {
        run = true;
        UdpClient client = new UdpClient();
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 5678);
        while (run)
        {
            await UpdateFaces(client, endpoint);
        }
    }

    private async Task UpdateFaces(UdpClient client, IPEndPoint endpoint)
    {
        client.Send(message, message.Length, endpoint);
        while (client.Available < 1)
        {
            await Task.Yield();
        }
        var data = new FrameData(client.Receive(ref endpoint));
        OnCenterFaceUpdated?.Invoke(data);
        Debug.Log(data);
    }

    void OnDestroy()
    {
        run = false;
    }

    public struct FrameData
    {
        public readonly DateTime timestamp;
        public readonly int width, height, centerIdx;
        public readonly Rect[] faces;

        public FrameData(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    timestamp = DateTime.FromBinary(br.ReadInt64());
                    width = br.ReadInt32();
                    height = br.ReadInt32();
                    centerIdx = br.ReadInt32();
                    int faceCount = (buffer.Length - 20) / 16;
                    faces = new Rect[faceCount];
                    Rect r = Rect.zero;
                    for (int i = 0; i < faceCount; i++)
                    {
                        r.x = br.ReadInt32();
                        r.y = br.ReadInt32();
                        r.width = br.ReadInt32();
                        r.height = br.ReadInt32();
                        faces[i] = r;
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{timestamp}\n{width} x {height}\n{centerIdx}";
        }
    }
}
