using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class MessageReceiver : MonoBehaviour
{
    public string Address = "/cheki";

    public OSCReceiver Receiver;

    private void Start()
    {
        Receiver.Bind(Address, ReceivedChekiMessage);
    }

    private void ReceivedChekiMessage(OSCMessage message)
    {
        var value = message.Values[0].BoolValue;
        
        if (value == true)
        {
            Debug.LogFormat("Received: {0}", value);
        }
    }

}
