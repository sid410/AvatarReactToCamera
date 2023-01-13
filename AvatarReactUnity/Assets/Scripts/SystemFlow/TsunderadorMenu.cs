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

    GameObject udpSender;
    UDPSender udpSenderScript;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        udpSender = GameObject.Find("UDPSender");
        udpSenderScript = udpSender.GetComponent<UDPSender>();

        state = new StateManager("TsunderadorFirst");
        udpSenderScript.State = state.getState();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && tsundereMode == "normal")
        {
            animator.SetBool("isStartNormal", true);
            Debug.Log("ちょっと！別にが大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから\n勘違いしないでこれ飲んだらとっとと帰ってよ！わかった");
            dialogue.text = "ちょっと！別にが大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから\n勘違いしないでこれ飲んだらとっとと帰ってよ！わかった";
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("NormalTsunTsun") && tsundereMode == "normal" && MotionTriger())
        {
            state.nextState();
            udpSenderScript.State = state.getState();
            animator.SetBool("isStartNormal", false);
            animator.SetBool("isConfirm", true);
            Debug.Log("何ではいとか言っちゃうのよ！はいとか言わずにずっとの側にいてね");
            dialogue.text = "何ではいとか言っちゃうのよ！はいとか言わずにずっとの側にいてね";
        }
    }

    bool MotionTriger()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
        else
            return false;
    }
}
