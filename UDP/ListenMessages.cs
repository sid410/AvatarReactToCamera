using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenMessages : MonoBehaviour
{
    private UDPHelper m_udpHelper;
    //private AnimationController m_UnitychanMove;
    private UnitychanMove m_UnitychanMove;
    private KaguyaMove m_KaguyaMove;
    private ManakaMove m_ManakaMove;

/*    [SerializeField]
    private GameObject unitychanObj;
    UnitychanMove unitychanScript;*/

    [SerializeField]
    private GameObject kaguyaObj;
    KaguyaMove kaguyaScript;

    [SerializeField]
    private GameObject manakaObj;
    ManakaMove manakaScript;



    private void Awake()
    {
        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        //m_UnitychanMove = GameObject.FindObjectOfType<AnimationController>();

/*        m_UnitychanMove = GameObject.FindObjectOfType<UnitychanMove>();
        unitychanScript = unitychanObj.GetComponent<UnitychanMove>();*/

        m_KaguyaMove = GameObject.FindObjectOfType<KaguyaMove>();
        kaguyaScript = kaguyaObj.GetComponent<KaguyaMove>();

        m_ManakaMove = GameObject.FindObjectOfType<ManakaMove>();
        manakaScript = manakaObj.GetComponent<ManakaMove>();
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
                if (kaguyaScript.calledObject)
                {
                    if (m_KaguyaMove.State != KaguyaMove.InteractionState.Start || m_KaguyaMove.Gesture != KaguyaMove.InteractionGesture.Idle) return;
                    if (splitMsg[1] == "Wave") m_KaguyaMove.Gesture = KaguyaMove.InteractionGesture.Wave;
                    if (splitMsg[1] == "Moe") m_KaguyaMove.Gesture = KaguyaMove.InteractionGesture.Moe;
                }
                else if (manakaScript.calledObject)
                {
                    if (m_ManakaMove.State != ManakaMove.InteractionState.Start || m_ManakaMove.Gesture != ManakaMove.InteractionGesture.Idle) return;
                    if (splitMsg[1] == "Wave") m_ManakaMove.Gesture = ManakaMove.InteractionGesture.Wave;
                    if (splitMsg[1] == "Moe") m_ManakaMove.Gesture = ManakaMove.InteractionGesture.Moe;
                }
/*                else if (unitychanScript.calledObject)
                {
                    if (m_UnitychanMove.State != UnitychanMove.InteractionState.Start || m_UnitychanMove.Gesture != UnitychanMove.InteractionGesture.Idle) return;
                    if (splitMsg[1] == "Wave") m_UnitychanMove.Gesture = UnitychanMove.InteractionGesture.Wave;
                    if (splitMsg[1] == "Moe") m_UnitychanMove.Gesture = UnitychanMove.InteractionGesture.Moe;
                }*/

                //if (splitMsg[1] == "Wave") m_UnitychanMove.MovePositionControllerMessage = "interactionHi";
                //if (splitMsg[1] == "Nyan") m_UnitychanMove.Gesture = UnitychanMove.InteractionGesture.Nyan;
                //if (splitMsg[1] == "Nico") m_UnitychanMove.Gesture = UnitychanMove.InteractionGesture.Nico;
                break;

            case "state":
                if (kaguyaScript.calledObject)
                {
                    if (splitMsg[1] == "InteractionStart" && m_KaguyaMove.State != KaguyaMove.InteractionState.Start) m_KaguyaMove.MovePositionControllerMessage = "goDestination";
                    if (splitMsg[1] == "InteractionStop" && m_KaguyaMove.State != KaguyaMove.InteractionState.Stop) m_KaguyaMove.MovePositionControllerMessage = "backStartPosition";
                }
                if (manakaScript.calledObject)
                {
                    if (splitMsg[1] == "InteractionStart" && m_ManakaMove.State != ManakaMove.InteractionState.Start) m_ManakaMove.MovePositionControllerMessage = "goDestination";
                    if (splitMsg[1] == "InteractionStop" && m_ManakaMove.State != ManakaMove.InteractionState.Stop) m_ManakaMove.MovePositionControllerMessage = "backStartPosition";
                }
/*                if (unitychanScript.calledObject)
                {
                    if (splitMsg[1] == "InteractionStart" && m_UnitychanMove.State != UnitychanMove.InteractionState.Start) m_UnitychanMove.MovePositionControllerMessage = "goDestination";
                    if (splitMsg[1] == "InteractionStop" && m_UnitychanMove.State != UnitychanMove.InteractionState.Stop) m_UnitychanMove.MovePositionControllerMessage = "backStartPosition";
                }*/
                break;

            default:
                Debug.Log("Message format error");
                break;
        }
    }

}
