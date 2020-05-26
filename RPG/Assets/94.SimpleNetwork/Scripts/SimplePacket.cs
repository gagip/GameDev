using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SimplePacket : MonoBehaviour {

    public float mouseX = 0.0f;
    public float mouseY = 0.0f;

    public static byte[] ToByteArray(SimplePacket packet)
    {
        MemoryStream stream = new MemoryStream();

        BinaryFormatter formatter = new BinaryFormatter();
        // mouse X, Y
        formatter.Serialize(stream, packet.mouseX);
        formatter.Serialize(stream, packet.mouseY);

        return stream.ToArray();
    }
}
