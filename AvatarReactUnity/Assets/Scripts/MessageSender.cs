using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class MessageSender : MonoBehaviour
{
    public string Address = "/cheki";

    public OSCTransmitter Transmitter;

    //private void Start()
    //{
    //    SendChekiStatus();
    //}

    public void SendChekiStatus()
	{
		var message = new OSCMessage(Address);
		message.AddValue(OSCValue.Bool(true));

		Transmitter.Send(message);
	}

}
