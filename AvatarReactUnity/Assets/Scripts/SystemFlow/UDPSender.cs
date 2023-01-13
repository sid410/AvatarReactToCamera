using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class UDPSender : MonoBehaviour
{
    // broadcast address
    public string host = "127.0.0.1";
    private int port = 10002;
    private UdpClient client;
    public string State;
    private int StateNumber;

    void Start()
    {
        client = new UdpClient();
        client.Connect(host, port);
    }


    void Update()
    {
        if(State == "Start") { StateNumber = 0; }
        else if (State == "Greeting") { StateNumber = 1; }
        else if (State == "TsunderadorFirst") { StateNumber = 2; }
        else if (State == "TsunderadorSecond") { StateNumber = 3; }
        else if (State == "Cheki") { StateNumber = 4; }
        else if (State == "Finish") { StateNumber = 5; }
        else { }
        Debug.Log(State);

        byte[] dgram = Encoding.UTF8.GetBytes(StateNumber.ToString());
        client.Send(dgram, dgram.Length);
    }

    
    void OnApplicationQuit()
    {
        client.Close();
    }
}
