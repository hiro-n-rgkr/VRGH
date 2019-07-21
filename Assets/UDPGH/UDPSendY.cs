using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class UDPSendY: MonoBehaviour
{
    // broadcast address
    public string host = "192.168.1.10";
    public int port = 4000;
    private UdpClient client;

    void Start ()
    {
        client = new UdpClient();
        client.Connect(host, port);
    }

    void Update ()
    {
        Vector3 tmp = GameObject.Find("Cube").transform.position;
        byte[] dgram = Encoding.UTF8.GetBytes(tmp.y.ToString());
        client.Send(dgram, dgram.Length);
    }

    // void OnGUI()
    // {
    //     if(GUI.Button (new Rect (10,10,100,40), "Send"))
    //     {
    //         byte[] dgram = Encoding.UTF8.GetBytes("hello!");
    //         client.Send(dgram, dgram.Length);
    //     }
    // }

    void OnApplicationQuit()
    {
        client.Close();
    }
}