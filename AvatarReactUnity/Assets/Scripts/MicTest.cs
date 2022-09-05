using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Text;
using System.Linq;

public class MicTest : MonoBehaviour
{
    public string message;

    private DictationRecognizer dictationRecognizer;
    // Start is called before the first frame update
    void Start()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        message = text;
        dictationRecognizer.Start();
    }
    private void DictationRecognizer_DictationHypothesis(string text)
    {
        // do something
    }
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        // do something
    }
}
