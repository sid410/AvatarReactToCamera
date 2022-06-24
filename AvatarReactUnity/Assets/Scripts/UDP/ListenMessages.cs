using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenMessages : MonoBehaviour
{
    private UDPHelper m_udpHelper;
    private UnitychanMove m_UnityChanMove;

    private void Awake()
    {
        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        m_UnityChanMove = GameObject.FindObjectOfType<UnitychanMove>();
    }

    private void OnEnable()
    {
        m_udpHelper.MessageReceived += UDPMessageReceived;
    }

    private void OnDisable()
    {
        m_udpHelper.MessageReceived -= UDPMessageReceived;
    }
    
    private void UDPMessageReceived(NetInMessage message)
    {
        string msg = message.ReadString();
        string[] splitMsg = msg.Split(char.Parse(":"));

        switch (splitMsg[0])
        {
            case "gesture":
                if (m_UnityChanMove.State != UnitychanMove.InteractionState.Start || m_UnityChanMove.Gesture != UnitychanMove.InteractionGesture.Finished) return;
                if (splitMsg[1] == "interactionHi") m_UnityChanMove.Gesture = UnitychanMove.InteractionGesture.Hi;
                break;

            case "state":
                if (splitMsg[1] == "InteractionStart" && m_UnityChanMove.State != UnitychanMove.InteractionState.Start) m_UnityChanMove.MovePositionControllerMessage = "goDestination";
                if (splitMsg[1] == "InteractionStop" && m_UnityChanMove.State != UnitychanMove.InteractionState.Stop) m_UnityChanMove.MovePositionControllerMessage = "backStartPosition";
                break;

            default:
                Debug.Log("Message format error");
                break;
        }
    }

}
