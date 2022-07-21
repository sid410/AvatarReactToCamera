using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenMessages : MonoBehaviour
{
    private UDPHelper m_udpHelper;
    //private AnimationController m_UnityChanMove;
    private UnitychanMove m_UnityChanMove;

    private void Awake()
    {
        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        //m_UnityChanMove = GameObject.FindObjectOfType<AnimationController>();
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
                if (m_UnityChanMove.State != UnitychanMove.InteractionState.Start || m_UnityChanMove.Gesture != UnitychanMove.InteractionGesture.Idle) return;
                if (splitMsg[1] == "Wave") m_UnityChanMove.Gesture = UnitychanMove.InteractionGesture.Wave;
                //if (splitMsg[1] == "Wave") m_UnityChanMove.MovePositionControllerMessage = "interactionHi";
                //if (splitMsg[1] == "Nyan") m_UnityChanMove.Gesture = UnitychanMove.InteractionGesture.Nyan;
                //if (splitMsg[1] == "Nico") m_UnityChanMove.Gesture = UnitychanMove.InteractionGesture.Nico;
                if (splitMsg[1] == "Moe") m_UnityChanMove.Gesture = UnitychanMove.InteractionGesture.Moe;
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
