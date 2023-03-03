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
    private DialogueManager dialogueManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        inputMessage = "";

        state = new StateManager("Start");
        isFinish = true;

        //sources = gameObject.GetComponents<AudioSource>();
        dialogueManager = new DialogueManager(dialogue, gameObject.GetComponents<AudioSource>());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("dialogueManager State :" + dialogueManager.getState());
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && MotionTriger() && tsundereMode == "normal")
        {
            state.nextState();
            animator.SetBool("isStartNormal", true);
            dialogueManager.activateAudio("Tsun");
            isFinish = false;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && MotionTriger() && tsundereMode == "normal")
        {
            state.nextState();
            animator.SetBool("isConfirm", true);
            dialogueManager.activateAudio("Dere");
            isFinish = false;
        }
        else if (dialogueManager.checkState() == "WaitAnswer")
        {
            animator.SetBool("isStartNormal", false);
            isFinish = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("NormalDereDere") && dialogueManager.getState() == "Stanby")
        {
            Debug.Log("Finish");
            state.nextState();
            animator.SetBool("isConfirm", false);
        }
    }

    bool MotionTriger()
    {
        if (state.getState() == "Start" && Input.GetKey(KeyCode.LeftShift)) return true;
        if (state.getState() == "TsunderadorFirst" && Input.GetKey(KeyCode.LeftControl)) return true;
        else return false;

        //if (state.getState() == "Start" && inputMessage == "state1") return true;
        //if (state.getState() == "TsunderadorFirst" && inputMessage == "state2") return true;
        //else return false;
    }


    private class DialogueManager
    {
        private enum DialogueState
        {
            Stanby,
            TsunTsunFirst,
            TsunTsunSecond,
            DereDereFirst,
            DereDereSecond,
        };

        TextMeshProUGUI dialogue;
        AudioSource[] sources;
        DialogueState state;

        public DialogueManager(TextMeshProUGUI dialogue, AudioSource[] sources)
        {
            this.dialogue = dialogue;
            this.sources = sources;
            this.state = DialogueState.Stanby;
        }

        public void activateAudio(string mode)
        {
            if (mode == "Tsun")
            {
                this.state = DialogueState.TsunTsunFirst;
                this.dialogue.text = "ちょっと！\n別に「かぐや」が大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから";
                Debug.Log("ちょっと！\n別に「かぐや」が大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから");
                this.sources[0].Play();
            }
            else if (mode == "Dere")
            {
                this.state = DialogueState.DereDereFirst;
                this.dialogue.text = "なんではいとか言っちゃうのよ！";
                Debug.Log("なんではいとか言っちゃうのよ！");
                this.sources[2].Play();
            }
        }

        public string checkState()
        {
            if (this.state == DialogueState.Stanby) 
                return "Stanby";
            else if (this.state == DialogueState.TsunTsunFirst && !this.sources[0].isPlaying)
            {
                this.state = DialogueState.TsunTsunSecond;
                this.dialogue.text = "勘違いしないでこれ飲んだらとっとと帰ってよ！\nわかった？";
                Debug.Log("勘違いしないでこれ飲んだらとっとと帰ってよ！\nわかった？");
                this.sources[1].Play();
                return "TsunTsunSecond";
            }
            else if (this.state == DialogueState.TsunTsunSecond && !this.sources[1].isPlaying)
                return "WaitAnswer";
            else if (this.state == DialogueState.DereDereFirst && !this.sources[2].isPlaying)
            {
                this.state = DialogueState.DereDereSecond;
                this.dialogue.text = "はいとか言わずにずっと「かぐや」の側にいてね";
                Debug.Log("はいとか言わずにずっと「かぐや」の側にいてね");
                this.sources[3].Play();
                return "DereDereSecond";
            }
            else if (this.state == DialogueState.DereDereSecond && !this.sources[3].isPlaying)
            {
                this.state = DialogueState.Stanby;
                this.dialogue.text = "ありがとうございました！";
                Debug.Log("ありがとうございました！");
                return "Finish";
            }
            else return "Error";
        }

        public string getState()
        {
            if (this.state == DialogueState.Stanby) return "Stanby";
            else if (this.state == DialogueState.TsunTsunFirst) return "TsunTsunFirst";
            else if (this.state == DialogueState.TsunTsunSecond) return "TsunTsunSecond";
            else if (this.state == DialogueState.DereDereFirst) return "DereDereFirst";
            else if (this.state == DialogueState.DereDereSecond) return "DereDereSecond";
            else return "cannot get state";
        }
    }
}
