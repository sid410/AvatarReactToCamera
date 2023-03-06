using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;
using System.Collections.Generic;

public class RecAudio : MonoBehaviour
{
    public GameObject Kaguya;
    public bool InputOn;
    public bool KeywordOn;
    public bool Finish_Rec;
    public string InputState;
    private int State;
    private string[] m_Keywords;
    
    private string[] m_Keywords_State1 = new string[] { "かぐや", "家具や", "カグヤ"};
    private string[] m_Keywords_State2 = new string[] { "おねがい", "お願い" };
    private string[] m_Keywords_State3 = new string[] { "うん", "はい" };
    private string[] m_Keywords_State4 = new string[] { "ありがとう" , "activate"};



    private KeywordRecognizer m_Recognizer;
    

    // Start is called before the first frame update
    
    void Start()
    {
        m_Keywords = m_Keywords_State1.Concat(m_Keywords_State2).ToArray();
        m_Keywords = m_Keywords.Concat(m_Keywords_State3).ToArray();
        m_Keywords = m_Keywords.Concat(m_Keywords_State4).ToArray();

        State = 0;
        Debug.Log("Start");
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }

        m_Recognizer = new KeywordRecognizer(m_Keywords);
        
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TsunderadarMove>().stateMessage = InputState;

    }
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());
        KeywordOn = true;
        if (0 <= Array.IndexOf(m_Keywords_State1, args.text) && State == 0 && GetComponent<TsunderadarMove>().isFinish == true)
        {
            State++;
            InputState = "state" + State.ToString();
        }
        if (0 <= Array.IndexOf(m_Keywords_State2, args.text) && State == 1 && GetComponent<TsunderadarMove>().isFinish == true)
        {
            State++;
            InputState = "state" + State.ToString();
        }
        if (0 <= Array.IndexOf(m_Keywords_State3, args.text) && State == 2 && GetComponent<TsunderadarMove>().isFinish == true)
        {
            State++;
            InputState = "state" + State.ToString();
        }
        if (0 <= Array.IndexOf(m_Keywords_State4, args.text) && State == 3 && GetComponent<TsunderadarMove>().isFinish == true)
        {
            State++;
            InputState = "state" + State.ToString();
            State = 0;
        }
    }

}
