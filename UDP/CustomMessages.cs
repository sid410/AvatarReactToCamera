using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

public class CustomMessages : Singleton<CustomMessages>
{
    public string destinationIP = "127.0.0.1";
    public int destinationPort = 11000;

    UDPHelper udpHelper;

    protected override void Awake()
    {
        udpHelper = GameObject.FindObjectOfType<UDPHelper>();
    }

    void Start()
    {
        InitializeMessageHandlers();
    }

    void InitializeMessageHandlers()
    {
    }

    public void StatusMessage()
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteString("Im Alive!!");
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
        else Debug.Log("no server connection!");
    }

    public void RaycastHitposMessage(Vector3 _ray)
    { 
        // add what to do with raypos here, call in PlaceonMesh.Place
    }

    public void SendTransformData(Vector3 pos, Vector3 rot)
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteVector3(pos);
            msg.WriteVector3(rot);
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
    }

    public void RotateMessage()
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteString("Rotate Cylinder clicked!");
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
        else Debug.Log("no server connection!");
    }

    public void HideMessage()
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteString("Hide Cylinder clicked!");
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
        else Debug.Log("no server connection!");
    }

    public void ShowMessage()
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteString("Show Cylinder clicked!");
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
        else Debug.Log("no server connection!");
    }

    public void NextMessage()
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteString("Next Cylinder clicked!");
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
        else Debug.Log("no server connection!");
    }

    public void PrevMessage()
    {
        if (this.udpHelper != null)
        {
            NetOutMessage msg = new NetOutMessage();
            msg.WriteString("Prev Cylinder clicked!");
            udpHelper.Send(msg, destinationIP, destinationPort);
        }
        else Debug.Log("no server connection!");
    }
}