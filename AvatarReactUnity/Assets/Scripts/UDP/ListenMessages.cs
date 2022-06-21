using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenMessages : MonoBehaviour
{
    private const float maxDepth = 6.0f;

    private UDPHelper m_udpHelper;
    private float _centerDepth;

    public GameObject capsuleGO;
    private MeshRenderer capsuleRenderer;
    private Animator capsuleAnimator;
    private Text textStatus;

    //for gradienting the normalized color
    private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;
    
    // ------------for sharing to other scripts------------
    public float CenterDepth
    {
        get { return _centerDepth;  }
        set { _centerDepth = value; }
    }
    public enum AppState
    {
        NotConnected, Searching, Calibrating, NoHand, Waving
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
    }

    private void Start()
    {
        SetGradiency();
        capsuleRenderer = capsuleGO.GetComponent<MeshRenderer>();
        capsuleAnimator = capsuleGO.GetComponent<Animator>();
        textStatus = GameObject.FindObjectOfType<Text>();
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

        if (splitMsg[0] == "status")
        {
            if (splitMsg[1] == "Waving" && State != AppState.Waving) capsuleAnimator.Play("Base Layer.WaveAnimation", -1, 0);

            if (splitMsg[1] == "Searching") ChangeState(AppState.Searching);
            if (splitMsg[1] == "Calibrating") ChangeState(AppState.Calibrating);
            if (splitMsg[1] == "NoHand") ChangeState(AppState.NoHand);
            if (splitMsg[1] == "Waving") ChangeState(AppState.Waving);

            textStatus.text = splitMsg[1];
        }

        if (splitMsg[0] == "depth")
        {
            if (float.TryParse(splitMsg[1], out _centerDepth))
            {
                if (!(_centerDepth > 0 && _centerDepth < maxDepth)) return;

                capsuleRenderer.material.color = gradient.Evaluate(_centerDepth / maxDepth);
            }
            else Debug.Log("Failed to parse");
        }
    }

    private void SetGradiency()
    {
        gradient = new Gradient();
        
        //from green to red
        colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.green;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.yellow;
        colorKey[1].time = 0.08f;
        colorKey[2].color = Color.red;
        colorKey[2].time = 1.0f;

        // always opaque
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }
}
