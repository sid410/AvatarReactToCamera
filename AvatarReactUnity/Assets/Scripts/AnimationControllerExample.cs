using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerExample : MonoBehaviour
{
    private ListenMessages m_ListenMessages;

    private void Start()
    {
        m_ListenMessages = FindObjectOfType<ListenMessages>();
    }

    //Example of how to call the public variables in ListenMessages
    private void Update()
    {
        Debug.Log(m_ListenMessages.State == ListenMessages.AppState.NotConnected);
        Debug.Log(m_ListenMessages.CenterDepth);
    }
}
