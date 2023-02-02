using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TsunderadorMenu : MonoBehaviour
{
    public TextMeshProUGUI dialogue;
    public string tsundereMode;
    public StateManager state;
    public string inputMessage;
    public bool isFinish;

    GameObject udpSender;
    UDPSender udpSenderScript;

    private Animator animator;
    private AudioSource[] sources;

    void Start()
    {
        animator = GetComponent<Animator>();
        //udpSender = GameObject.Find("UDPSender");
        //udpSenderScript = udpSender.GetComponent<UDPSender>();
        //udpSenderScript.State = state.getState();
        inputMessage = "";

        state = new StateManager("Start");
        isFinish = true;

        sources = gameObject.GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MotionTriger() && tsundereMode == "normal")
        {
            state.nextState();
            animator.SetBool("isStartNormal", true);
            Debug.Log("ちょっと！別に「かぐや」が大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから\n勘違いしないでこれ飲んだらとっとと帰ってよ！わかった？");
            dialogue.text = "ちょっと！別に「かぐや」が大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから\n勘違いしないでこれ飲んだらとっとと帰ってよ！わかった？";
            sources[0].Play();
            isFinish = false;
        }
        else if (MotionTriger() && tsundereMode == "normal")
        {
            state.nextState();
            animator.SetBool("isConfirm", true);
            Debug.Log("なんではいとか言っちゃうのよ！はいとか言わずにずっと「かぐや」の側にいてね");
            dialogue.text = "なんではいとか言っちゃうのよ！はいとか言わずにずっと「かぐや」の側にいてね";
            sources[1].Play();
            isFinish = false;
        }
        else if (!sources[0].isPlaying)
        {
            animator.SetBool("isStartNormal", false);
            isFinish = true;
        }
    }

    bool MotionTriger()
    {
        if (state.getState() == "Start" && inputMessage == "state1") return true;
        else if (state.getState() == "TsunderadorFirst" && inputMessage == "state2") return true;
        else return false;
    }
}
