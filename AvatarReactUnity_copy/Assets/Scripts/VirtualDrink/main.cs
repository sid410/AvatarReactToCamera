using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class main : MonoBehaviour
{
    public int agentNum;
    public GameObject agent;
    public TextMeshProUGUI dialogue;
    public string mode;
    public StateManager state;
    public string inputMessage;
    public bool isFinish;

    private Animator animator;
    private DialogueManager dialogueManager;

    void Start()
    {
        agentNum = 0;//0:Kaguya, 1:Manaka
        mode = "TSUNDERE";
        if (agent == null) agent = GameObject.Find("Agent");
        animator = agent.transform.GetChild(agentNum).GetComponent<Animator>();

        inputMessage = "";

        state = new StateManager("Start");
        isFinish = true;

        //sources = gameObject.GetComponents<AudioSource>();
        if (dialogue == null) Debug.Log("dialogue is null.");
        Debug.Log(gameObject.GetComponent<AudioSource>());
        dialogueManager = new DialogueManager(dialogue, gameObject.GetComponent<AudioSource>());

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("dialogueManager State :" + dialogueManager.getState());
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && MotionTriger() && mode == "TSUNDERE")
        {
            state.nextState();
            animator.SetBool("isStartNormal", true);
            dialogueManager.activateAudio("Tsun");
            isFinish = false;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && MotionTriger() && mode == "TSUNDERE")
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetAgent();
        }
    }

    bool MotionTriger()
    {
        //if (state.getState() == "Start" && Input.GetKey(KeyCode.LeftShift)) return true;
        //if (state.getState() == "TsunderadorFirst" && Input.GetKey(KeyCode.LeftControl)) return true;
        //else return false;

        if (state.getState() == "Start" && inputMessage == "state1") return true;
        if (state.getState() == "TsunderadorFirst" && inputMessage == "state2") return true;
        else return false;
    }

    private void SetAgent()
    {
        int prevAgentNum = agentNum;
        switch (agentNum)
        {
            case 0:
                agentNum++;//Change to Manaka
                break;
            case 1: 
                agentNum = 0;//Change to Kaguya
                break;
            default:
                agentNum = 0;
                break;
        }
        if (agent == null) agent = GameObject.Find("Agent");
        agent.transform.GetChild(prevAgentNum).gameObject.SetActive(false);//前のagentを非表示
        agent.transform.GetChild(agentNum).gameObject.SetActive(true);//次のagentを表示
        animator = agent.transform.GetChild(agentNum).GetComponent<Animator>();
    }

    private class DialogueManager
    {
        private enum DialogueState
        {
            Stanby,
            TSUNDERE_01,
            TSUNDERE_02,
            TSUNDERE_03,
            TSUNDERE_04,
        };

        TextMeshProUGUI dialogue;
        AudioSource audioSource;
        DialogueState state;
        AudioClip[] voices;

        public DialogueManager(TextMeshProUGUI dialogue, AudioSource audioSource)
        {
            this.dialogue = dialogue;
            this.audioSource = audioSource;
            this.state = DialogueState.Stanby;
            //Resourcesから音声ファイルの抽出
            string folderName_TD = "TSUNDERE";//Voice/TSUNDERE内のファイルを取得
            UnityEngine.Object[] audioClips_TD = Resources.LoadAll("Voices/" + folderName_TD, typeof(AudioClip));
            string folderName_DD = "DEREDERE";//Voice/DEREDERE内のファイルを取得
            UnityEngine.Object[] audioClips_DD = Resources.LoadAll("Voices/" + folderName_DD, typeof(AudioClip));
            voices = new AudioClip[audioClips_TD.Length + audioClips_DD.Length];//ファイルサイズの設定
            for (int i = 0; i < audioClips_TD.Length; i++) this.voices[i] = Resources.Load("Voices/" + folderName_TD + "/" + audioClips_TD[i].name, typeof(AudioClip)) as AudioClip;
            for (int i = audioClips_TD.Length; i < audioClips_TD.Length + audioClips_DD.Length; i++) this.voices[i] = Resources.Load("Voices/" + folderName_DD + "/" + audioClips_DD[i].name, typeof(AudioClip)) as AudioClip;
        }

        public void activateAudio(string mode)
        {
            if (mode == "Tsun")
            {
                this.state = DialogueState.TSUNDERE_01;
                this.dialogue.text = "ちょっと！\n別に「かぐや」が大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから";
                Debug.Log("ちょっと！\n別に「かぐや」が大好きなご主人様のために持ってきたわけだとか全然そんなんじゃないんだから");
                this.audioSource.PlayOneShot(this.voices[0]);//0
            }
            else if (mode == "Dere")
            {
                this.state = DialogueState.TSUNDERE_03;
                this.dialogue.text = "なんではいとか言っちゃうのよ！";
                Debug.Log("なんではいとか言っちゃうのよ！");
                this.audioSource.PlayOneShot(this.voices[2]);//2
            }
        }

        public string checkState()
        {
            if (this.state == DialogueState.Stanby)
                return "Stanby";
            else if (this.state == DialogueState.TSUNDERE_01 && !this.audioSource.isPlaying)//0
            {
                this.state = DialogueState.TSUNDERE_02;
                this.dialogue.text = "勘違いしないでこれ飲んだらとっとと帰ってよ！\nわかった？";
                Debug.Log("勘違いしないでこれ飲んだらとっとと帰ってよ！\nわかった？");
                this.audioSource.PlayOneShot(this.voices[1]);//1
                return "TSUNDERE_02";
            }
            else if (this.state == DialogueState.TSUNDERE_02 && !this.audioSource.isPlaying)//1
                return "WaitAnswer";
            else if (this.state == DialogueState.TSUNDERE_03 && !this.audioSource.isPlaying)//2
            {
                this.state = DialogueState.TSUNDERE_04;
                this.dialogue.text = "はいとか言わずにずっと「かぐや」の側にいてね";
                Debug.Log("はいとか言わずにずっと「かぐや」の側にいてね");
                this.audioSource.PlayOneShot(this.voices[3]);//3
                return "TSUNDERE_04";
            }
            else if (this.state == DialogueState.TSUNDERE_04 && !this.audioSource.isPlaying)//3
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
            else if (this.state == DialogueState.TSUNDERE_01) return "TSUNDERE_01";
            else if (this.state == DialogueState.TSUNDERE_02) return "TSUNDERE_02";
            else if (this.state == DialogueState.TSUNDERE_03) return "TSUNDERE_03";
            else if (this.state == DialogueState.TSUNDERE_04) return "TSUNDERE_04";
            else return "cannot get state";
        }
    }
}
