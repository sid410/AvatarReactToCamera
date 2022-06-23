using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenMessages : MonoBehaviour
{
    private const float maxDepth = 6.0f;

    private UDPHelper m_udpHelper;
    private float _centerDepth = 0.0f;


    ////for gradienting the normalized color
    //private Gradient gradient;
    //private GradientColorKey[] colorKey;
    //private GradientAlphaKey[] alphaKey;

    // although better to use AppState, for now use Taguchi's public message variable
    private UnitychanMove m_UnityChanMove;
    
    // ------------for sharing to other scripts------------
    public float CenterDepth
    {
        get { return _centerDepth;  }
        set { _centerDepth = value; }
    }
    public enum AppState
    {
        Exited, Entered
    }
    public AppState State
    {
        get;
        set;
    }
    private void ChangeState(AppState newState)
    {
        if (State != newState)
        {
            State = newState;
        }
    }
    // ------------for sharing to other scripts------------

    private void Awake()
    {
        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

        // improve how to send data to Taguchi implementation later
        m_UnityChanMove = GameObject.FindObjectOfType<UnitychanMove>();
    }

    private void Start()
    {
        //SetGradiency();
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

        // improve how to send data to Taguchi implementation later
        if (splitMsg[0] == "status")
        {
            if (splitMsg[1] == "Waving") m_UnityChanMove.message = "interactionHi";
        }

        if (splitMsg[0] == "depth")
        {
            if (float.TryParse(splitMsg[1], out _centerDepth))
            {
                if (!(_centerDepth > 0 && _centerDepth < maxDepth)) return;

                else if (_centerDepth < 1.1f && State == AppState.Exited)
                {
                    m_UnityChanMove.message = "goDistination";
                    ChangeState(AppState.Entered);
                }

                else if (_centerDepth > 1.1f && State == AppState.Entered)
                {
                    m_UnityChanMove.message = "backStartPosition";
                    ChangeState(AppState.Exited);
                }
            }
            else Debug.Log("Failed to parse");
        }
    }

    //private void SetGradiency()
    //{
    //    gradient = new Gradient();
        
    //    //from green to red
    //    colorKey = new GradientColorKey[3];
    //    colorKey[0].color = Color.green;
    //    colorKey[0].time = 0.0f;
    //    colorKey[1].color = Color.yellow;
    //    colorKey[1].time = 0.08f;
    //    colorKey[2].color = Color.red;
    //    colorKey[2].time = 1.0f;

    //    // always opaque
    //    alphaKey = new GradientAlphaKey[2];
    //    alphaKey[0].alpha = 1.0f;
    //    alphaKey[0].time = 0.0f;
    //    alphaKey[1].alpha = 1.0f;
    //    alphaKey[1].time = 1.0f;

    //    gradient.SetKeys(colorKey, alphaKey);
    //}
}
