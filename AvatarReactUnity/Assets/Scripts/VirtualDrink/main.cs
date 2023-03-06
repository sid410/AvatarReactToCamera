using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class main : MonoBehaviour
{
    public int agentNum;
    public GameObject agent;
    public TextMeshProUGUI dialogue, checkText;
    public string mode;
    public StateManager_v2 state;
    public string inputMessage;
    public bool isFinish;

    private Animator animator;
    private DialogueManager dialogueManager;
    private ChangeDialogue changeDialogue;
    public CameraShake shake;//カメラを揺らすためのもの

    void Start()
    {
        agentNum = 0;//0:Kaguya, 1:Manaka
        mode = "TSUNDERE";
        if (agent == null) agent = GameObject.Find("Agent");
        animator = agent.transform.GetChild(agentNum).GetComponent<Animator>();

        inputMessage = "";

        state = new StateManager_v2("Stanby");
        isFinish = true;

        //sources = gameObject.GetComponents<AudioSource>();
        Debug.Log(gameObject.GetComponent<AudioSource>());
        dialogueManager = new DialogueManager(dialogue, gameObject.GetComponent<AudioSource>());
        if (changeDialogue == null) changeDialogue = gameObject.GetComponent<ChangeDialogue>();
        if (checkText == null) checkText = GameObject.Find("Canvas/CheckText").GetComponent<TextMeshProUGUI>();
        if (shake == null) shake = Camera.main.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        //Animationの遷移
        //AnimationState: Standby && mode: TSUNDERE
        checkText.text = "*Space key: Change virtual agent" + "\n"
            + "*Enter key: Shake Camera" + "\n"
            + "State: " + state.getState() + "\n"
            + "DialogueManager State: " + dialogueManager.getDialogueState() + "\n"
            + "Mode: " + mode + "\n";
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && MotionTriger())
        {
            state.nextState();
            animator.SetBool("isStartNormal", true);
            dialogueManager.activateAudio(this.state.getState(), mode);
            isFinish = false;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && MotionTriger())
        {
            state.nextState();
            animator.SetBool("isConfirm", true);
            dialogueManager.activateAudio(this.state.getState(), mode);
            isFinish = false;
        }
        else if (dialogueManager.checkState(mode) == "WaitAnswer")
        {
            animator.SetBool("isStartNormal", false);
            isFinish = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Second2") && dialogueManager.getDialogueState() == "Stanby")
        {
            Debug.Log("Finish");
            state.ResetState();//強制的にstate=Stanby
            StartCoroutine(DisplayText());
            animator.SetBool("isConfirm", false);
        }
        
        if (Input.GetKeyDown(KeyCode.Space)&& animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby"))
        {
            SetAgent();
        }
        if (Input.GetKey(KeyCode.Return))//カメラを揺らす
        {
            StartCoroutine(ShakeCamera());
        }
    }

    //マイク入力によるトリガー
    bool MotionTriger()
    {
        //if (state.getState() == "Start" && Input.GetKey(KeyCode.LeftShift)) return true;
        //if (state.getState() == "TsunderadorFirst" && Input.GetKey(KeyCode.LeftControl)) return true;
        //else return false;
        if (state.getState() == "Stanby" && inputMessage == "state1") return true;
        if (state.getState() == "First" && inputMessage == "state2") return true;
        else return false;
    }

    //Agentの表示を切り替える関数
    private void SetAgent()
    {
        int prevAgentNum = agentNum;
        switch (agentNum)
        {
            case 0:
                agentNum++;//Change to Manaka
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                break;
            case 1: 
                agentNum = 0;//Change to Kaguya
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                break;
            default:
                agentNum = 0;//Change to Kaguya
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                break;
        }
        if (agent == null) agent = GameObject.Find("Agent");
        agent.transform.GetChild(prevAgentNum).gameObject.SetActive(false);//前のagentを非表示
        agent.transform.GetChild(agentNum).gameObject.SetActive(true);//次のagentを表示
        animator = agent.transform.GetChild(agentNum).GetComponent<Animator>();
        state.ResetState();
        dialogueManager.ResetDialogueState();
        if (mode == "TSUNDERE") mode = "DEREDERE";
        else mode = "TSUNDERE";

    }

    IEnumerator  DisplayText()
    {
        yield return new WaitForSecondsRealtime(2f);
        dialogueManager.DisplayDialogueThanksText();
        yield return new WaitForSecondsRealtime(4f);
        dialogueManager.DisplayDialogueStanbyText();
    }
    //カメラを揺らす関数
    IEnumerator ShakeCamera()
    {
        float time = 0.1f;
        shake.Shake(time, 0.03f);
        yield return new WaitForSecondsRealtime(time);
        Camera.main.transform.position = new Vector3(-2.45f, 1.3f, -10.85f);//カメラを初期位置に戻す
        //Debug.Log("CameraPos: " + Camera.main.transform.position);
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
        DialogueState state; //状態変数？
        AudioClip[] tsunVoices, dereVoices; //音声ファイル
        string[] tsunTexts, dereTexts;//Dialogueファイル

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
            tsunVoices = new AudioClip[audioClips_TD.Length];//ファイルサイズの設定
            dereVoices = new AudioClip[audioClips_DD.Length];//ファイルサイズの設定
            for (int i = 0; i < audioClips_TD.Length; i++) this.tsunVoices[i] = Resources.Load("Voices/" + folderName_TD + "/" + audioClips_TD[i].name, typeof(AudioClip)) as AudioClip;
            for (int i = 0; i < audioClips_DD.Length; i++) this.dereVoices[i] = Resources.Load("Voices/" + folderName_DD + "/" + audioClips_DD[i].name, typeof(AudioClip)) as AudioClip;
            //TSUNDERE.txtデータの読み込み
            TextAsset textassetTD = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
            textassetTD = Resources.Load("Dialogues/TSUNDERE", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
            int tsunSize = textassetTD.text.Split('\n').Length;
            tsunTexts = new string[tsunSize];
            string[] textMessageTD = textassetTD.text.Split('\n');
            for (int i = 0; i < tsunSize; i++) tsunTexts[i] = textMessageTD[i].Replace(" ", "\n");//半角スペースを改行に変換
            //DEREDERE.txtデータの読み込み
            TextAsset textassetDD = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
            textassetDD = Resources.Load("Dialogues/DEREDERE", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
            int dereSize = textassetDD.text.Split('\n').Length;
            dereTexts = new string[dereSize];
            string[] textMessageDD = textassetDD.text.Split('\n');
            for (int i = 0; i < dereSize; i++) dereTexts[i] = textMessageDD[i].Replace(" ", "\n");//半角スペースを改行に変換
        }

        public void activateAudio(string state, string mode)
        {
            if (state == "First")
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.TSUNDERE_01;
                        this.dialogue.text = tsunTexts[0];
                        Debug.Log(tsunTexts[0]);
                        this.audioSource.PlayOneShot(this.tsunVoices[0]);//0
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_01;
                        this.dialogue.text = dereTexts[0];
                        Debug.Log(dereTexts[0]);
                        this.audioSource.PlayOneShot(this.dereVoices[0]);//0
                        break;
                    default:
                        break;
                }
                
            }
            else if (state == "Second")
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.TSUNDERE_03;
                        this.dialogue.text = tsunTexts[2];
                        Debug.Log(tsunTexts[2]);
                        this.audioSource.PlayOneShot(this.tsunVoices[2]);//2
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_03;
                        this.dialogue.text = dereTexts[2];
                        Debug.Log(dereTexts[2]);
                        this.audioSource.PlayOneShot(this.dereVoices[2]);//2
                        break;
                    default:
                        break;
                }
                
            }
        }

        public string checkState(string mode)
        {
            if (this.state == DialogueState.Stanby)
                return "Stanby";
            else if (this.state == DialogueState.TSUNDERE_01 && !this.audioSource.isPlaying)//0
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.TSUNDERE_02;
                        this.dialogue.text = tsunTexts[1];
                        Debug.Log(tsunTexts[1]);
                        this.audioSource.PlayOneShot(this.tsunVoices[1]);//1
                        return "TSUNDERE_02";
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_02;
                        this.dialogue.text = dereTexts[1];
                        Debug.Log(dereTexts[1]);
                        this.audioSource.PlayOneShot(this.dereVoices[1]);//1
                        return "TSUNDERE_02";
                        return "";
                        break;
                    default:
                        return "";
                        break;
                }
            }
            else if (this.state == DialogueState.TSUNDERE_02 && !this.audioSource.isPlaying)//1
                return "WaitAnswer";
            else if (this.state == DialogueState.TSUNDERE_03 && !this.audioSource.isPlaying)//2
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.TSUNDERE_04;
                        this.dialogue.text = tsunTexts[3];
                        Debug.Log(tsunTexts[3]);
                        this.audioSource.PlayOneShot(this.tsunVoices[3]);//3
                        return "TSUNDERE_04";
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_04;
                        this.dialogue.text = dereTexts[3];
                        Debug.Log(dereTexts[3]);
                        this.audioSource.PlayOneShot(this.dereVoices[3]);//3
                        return "TSUNDERE_04";
                        return "";
                        break;
                    default:
                        return "";
                        break;
                } 
            }
            else if (this.state == DialogueState.TSUNDERE_04 && !this.audioSource.isPlaying)//3
            {
                this.state = DialogueState.Stanby;
                //this.dialogue.text = tsunTexts[4];
                //Debug.Log(tsunTexts[4]);
                return "Finish";
            }
            else return "Error";
        }
       

        public string getDialogueState()
        {
            switch (this.state)
            {
                case DialogueState.Stanby:
                    return "Stanby";
                    break;
                case DialogueState.TSUNDERE_01:
                    return "TSUNDERE_01";
                    break;
                case DialogueState.TSUNDERE_02:
                    return "TSUNDERE_02";
                    break;
                case DialogueState.TSUNDERE_03:
                    return "TSUNDERE_03";
                    break;
                case DialogueState.TSUNDERE_04:
                    return "TSUNDERE_04";
                    break;
                default:
                    return "cannot get state";
                    break;
            }
        }
        public void ResetDialogueState()//強制的にstate=Stanby
        {
            this.state = DialogueState.Stanby;
        }
        public void DisplayDialogueThanksText()//Stanby時に表示
        {
            this.dialogue.text = "ありがとうございました！";
        }
        public void DisplayDialogueStanbyText()//Stanby時に表示
        {
            this.dialogue.text = "スタンバイ中...";
        }
    }
}

