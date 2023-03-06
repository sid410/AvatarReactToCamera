using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;
using System.Collections.Generic;

public class SoundRecognizer_v2 : MonoBehaviour
{
    public main main;
    public bool InputOn;
    public bool KeywordOn;
    public bool Finish_Rec;
    public string InputState;
    private int State;
    private string[] m_Keywords;

    private string[] m_Keywords_State1 = new string[] {"お願いします", "オネガイシマス", "スタート", "ツンデレ", "レーダー"};
    private string[] m_Keywords_State2 = new string[] {"はい","ハイ"};



    private KeywordRecognizer m_Recognizer;


    // Start is called before the first frame update

    void Start()
    {
        if(main==null)main = GameObject.Find("Main").GetComponent<main>();
        m_Keywords = m_Keywords_State1.Concat(m_Keywords_State2).ToArray();

        State = 0;
        //Debug.Log("Start");
        foreach (var device in Microphone.devices)
        {
            Debug.Log("DeviceName: " + device);
        }

        m_Recognizer = new KeywordRecognizer(m_Keywords);

        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();

    }

    // Update is called once per frame
    void Update()
    {
        main.inputMessage = InputState;
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());

        if (0 <= Array.IndexOf(m_Keywords_State1, args.text) && State == 0)
        {
            State++;
            InputState = "state" + State.ToString();
            Debug.Log(InputState);
        }
        if (0 <= Array.IndexOf(m_Keywords_State2, args.text) && State == 1)
        {
            State++;
            InputState = "state" + State.ToString();
            State = 0;
            Debug.Log(InputState);
        }
    }

}
