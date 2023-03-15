using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainV2 : MonoBehaviour
{
    public int agentNum;
    public GameObject agents;//AgentObject
    private GameObject agent;
    public TextMeshProUGUI dialogue, checkText;
    public string mode;
    public StateManager_v2 state;
    public string inputMessage;
    public bool isFinish;
    public Vector3 stanbyPosition;
    public Quaternion stanbyAngle;
    public Vector3 startPosition;
    public Quaternion startAngle;
    public Vector3 finishPosition;
    public Quaternion finishAngle;
    public float reviewTime;
    public float waitFeedback;
    public bool isReview, isMove, isCheki;
    private Animator animator;
    private DialogueManager dialogueManager;
    private ChangeDialogue changeDialogue;
    private MicRecorder _micRecorder;
    public CameraShake shake;//カメラを揺らすためのもの


    void Start()
    {
        agentNum = 0;//0:Kaguya, 1:Manaka
        if (agentNum == 0) mode = "TSUNDERE";
        else if (agentNum == 1) mode = "DEREDERE";
        if (agents == null) agents = GameObject.Find("Agent");
        agent = agents.transform.GetChild(agentNum).gameObject;
        //agentを待機場所に移動させる
        agent.transform.position = stanbyPosition;
        agent.transform.rotation = stanbyAngle;
        animator = agent.GetComponent<Animator>();

        inputMessage = "";

        state = new StateManager_v2("Stanby");
        isFinish = true;
        isReview = false;
        isMove = false;
        _micRecorder = (new GameObject("EventSystem")).AddComponent<MicRecorder>();
        dialogueManager = new DialogueManager(dialogue, gameObject.GetComponent<AudioSource>());
        if (changeDialogue == null) changeDialogue = gameObject.GetComponent<ChangeDialogue>();
        if (checkText == null) checkText = GameObject.Find("Canvas/CheckText").GetComponent<TextMeshProUGUI>();
        if (shake == null) shake = Camera.main.GetComponent<CameraShake>();

        state.ResetState();
        dialogueManager.ResetDialogueState();
        dialogueManager.DisplayDialogueStanbyText();

        animator.SetBool("isCheki", isCheki);

    }

    // Update is called once per frame
    void Update()
    {
        //if (agentNum == 0) mode = "TSUNDERE";
        //else if (agentNum == 1) mode = "DEREDERE";
        //Animationの遷移
        //AnimationState: Standby && mode: TSUNDERE
        checkText.text = "*Space key: Change virtual agent" + "\n"
            + "*Enter key: Shake Camera" + "\n"
            + "*N key: Change virtual agent accorfing to questionnaire" + "\n"
            + "State: " + state.getState() + "\n"
            + "DialogueManager State: " + dialogueManager.getDialogueState() + "\n"
            + "Mode: " + mode + "\n";
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wait") && MotionTriger())//End waiting
        {
            state.nextState();
            animator.SetBool("isStart", true); //Go to State:Move
            isFinish = false;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wait") && isFinish)//agent is waiting.
        {
            state.ResetState();
            dialogueManager.ResetDialogueState();
            dialogueManager.DisplayDialogueStanbyText();
            isFinish = true;
            Debug.Log("Agent is waiting...");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Standby") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.0f)//agent is comming.
        {
            //agent.transform.position = startPosition;
            //agent.transform.rotation = stanbyAngle;
            Debug.Log("Agent is comming");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && this.state.getState() == "Start")
        {
            dialogueManager.activateAudio(this.state.getState(), mode);
            animator.SetBool("isStart", false);//State:WaitのときにState:Moveに遷移させない様falseへ変更
            Debug.Log("Agent is started.");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("First") && dialogueManager.checkState(mode) == "TSUNDERE_01")//agent is behaving first.
        {
            //state.nextState();//Go to State:First
            dialogueManager.activateAudio(this.state.getState(), mode);
            isFinish = false;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && MotionTriger())//When user answers
        {
            state.nextState();
            animator.SetBool("isConfirm", true);//Go to State: Second
            dialogueManager.activateAudio(this.state.getState(), mode);
            isFinish = false;
            Debug.Log("Agent is started.");
        }
        else if (dialogueManager.checkState(mode) == "WaitAnswer")
        {
            animator.SetBool("isStartNormal", false);//State:StanbyのときにState:Firstに遷移させない様falseへ変更
            isFinish = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Second") && dialogueManager.getDialogueState() == "Stanby")
        {
            //Debug.Log("Finish");
            //state.nextState();//強制的にstate=Stanby
            //StartCoroutine(DisplayText());

            animator.SetBool("isConfirm", false);//State:WaitAnswerのときにState:Secondに遷移させない様falseへ変更
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bye") || animator.GetCurrentAnimatorStateInfo(0).IsName("Bye_Cheki"))
        {
            dialogueManager.DisplayDialogueThanksText();
        }
        //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Warp2") && !animator.GetBool("isWarp"))
        //{
        //    agent.transform.position = GameObject.Find("FinishPoint").transform.position;
        //    agent.transform.rotation = GameObject.Find("FinishPoint").transform.rotation;
        //    animator.SetBool("isWarp", true);
        //    dialogueManager.DisplayDialogueStanbyText();
        //}
        //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && dialogueManager.getDialogueState() == "Finish")
        //{
        //    state.ResetState();
        //    dialogueManager.ResetDialogueState();
        //    dialogueManager.DisplayDialogueStanbyText();
        //    isFinish = false;
        //    animator.SetBool("isOut", false);
        //    agent.transform.position = startPosition;
        //}
        if (Input.GetKey(KeyCode.N) && animator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))
        {
            pythonProgram py = this.GetComponent<pythonProgram>();
            py.ChangeBehaviourBasedOnQuestionnaire();
        }
        if (Input.GetKeyDown(KeyCode.Space) && animator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))
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
        if (state.getState() == "Stanby" && (inputMessage == "stanby" || Input.GetKey(KeyCode.S))) return true;
        if (state.getState() == "Start" && (inputMessage == "state1" || Input.GetKey(KeyCode.LeftShift))) return true;
        if (state.getState() == "First" && (inputMessage == "state2" || Input.GetKey(KeyCode.LeftControl))) return true;
        else return false;
    }

    //Agentの表示を切り替える関数
    public void SetAgent()
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
        if (agents == null) agents = GameObject.Find("Agent");
        agents.transform.GetChild(prevAgentNum).gameObject.SetActive(false);//前のagentを非表示
        agents.transform.GetChild(agentNum).gameObject.SetActive(true);//次のagentを表示
        agent = agents.transform.GetChild(agentNum).gameObject;
        //agentを待機場所に移動させる
        agent.transform.position = stanbyPosition;
        agent.transform.rotation = stanbyAngle;
        animator = agent.GetComponent<Animator>();
        state.ResetState();
        dialogueManager.ResetDialogueState();
        if (mode == "TSUNDERE") mode = "DEREDERE";
        else mode = "TSUNDERE";

    }

    IEnumerator DisplayText()
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

    public void SetAnimationState(string animationState)
    {
        switch (animationState)
        {
            case "Wait":
                Debug.Log("State: Wait");
                break;
            case "Move":
                Debug.Log("State: Move");
                break;
            case "Stanby":
                Debug.Log("State: Stanby");
                break;
            case "First":
                state.nextState();
                Debug.Log("State: First");
                break;
            case "WaitAnswer":
                Debug.Log("State: WaitAnswer");
                break;
            case "Second":
                Debug.Log("State: Second");
                break;
            case "Bye":
                Debug.Log("State: Bye");
                break;
            case "ByeCheki":
                Debug.Log("State: ByeCheki");
                break;
            case "Back":
                Debug.Log("State: Back");
                break;
            default:
                break;
        }
    }

    IEnumerator StartReview(float delay_time, DialogueManager dialogueManager, string mode)
    {
        yield return new WaitForSecondsRealtime(delay_time);

        dialogueManager.activateAudio("AfterTalk", mode);
        activateMic();
    }

    IEnumerator WaitFeedback(float delay_time, DialogueManager dialogueManager, string mode)
    {
        yield return new WaitForSecondsRealtime(delay_time);

        deactivateMic();
        dialogueManager.activateAudio("Response", mode);
    }

    private void activateMic()
    {
        _micRecorder.StartRecord();
        Debug.Log("Activate Mic");
    }

    private void deactivateMic()
    {
        if (_micRecorder.IsRecording)
            StartCoroutine(_micRecorder.StopRecord());
        Debug.Log("Deactivate Mic");
    }

    private class DialogueManager
    {

        private enum DialogueState
        {
            Stanby,
            Greeting,
            TSUNDERE_01,
            TSUNDERE_02,
            TSUNDERE_03,
            TSUNDERE_04,
            ThankYou,
            Finish,
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
            Debug.Log(state + ":" + mode);
            if (state == "Start")
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.Greeting;
                        this.dialogue.text = tsunTexts[0];
                        Debug.Log("this.dialogue.text: " + this.dialogue.text);
                        Debug.Log(tsunTexts[0]);
                        //this.audioSource.PlayOneShot(this.tsunVoices[0]);//0
                        this.audioSource.clip = this.tsunVoices[0];
                        this.audioSource.Play();
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.Greeting;
                        this.dialogue.text = dereTexts[0];
                        Debug.Log(dereTexts[0]);
                        //this.audioSource.PlayOneShot(this.dereVoices[0]);//0
                        this.audioSource.clip = this.dereVoices[0];
                        this.audioSource.Play();
                        break;
                    default:
                        break;
                }
            }

            else if (state == "First")
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.TSUNDERE_01;
                        this.dialogue.text = tsunTexts[1];
                        Debug.Log("this.dialogue.text: " + this.dialogue.text);
                        Debug.Log(tsunTexts[1]);
                        //this.audioSource.PlayOneShot(this.tsunVoices[0]);//0
                        this.audioSource.clip = this.tsunVoices[1];
                        this.audioSource.Play();
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_01;
                        this.dialogue.text = dereTexts[1];
                        Debug.Log(dereTexts[0]);
                        //this.audioSource.PlayOneShot(this.dereVoices[0]);//0
                        this.audioSource.clip = this.dereVoices[1];
                        this.audioSource.Play();
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
                        this.dialogue.text = tsunTexts[3];
                        Debug.Log(tsunTexts[3]);
                        //this.audioSource.PlayOneShot(this.tsunVoices[2]);//2
                        this.audioSource.clip = this.tsunVoices[3];
                        this.audioSource.Play();
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_03;
                        this.dialogue.text = dereTexts[3];
                        Debug.Log(dereTexts[3]);
                        //this.audioSource.PlayOneShot(this.dereVoices[2]);//2
                        this.audioSource.clip = this.dereVoices[3];
                        this.audioSource.Play();
                        break;
                    default:
                        break;
                }
            }
        }

        public string checkState(string mode)
        {
            //Debug.Log("audioSource.isPlaying: " + this.audioSource.isPlaying);
            if (this.state == DialogueState.Stanby)
                return "Stanby";
            else if (this.state == DialogueState.Greeting && !this.audioSource.isPlaying)
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        return "TSUNDERE_01";
                        break;
                    case "DEREDERE":
                        return "TSUNDERE_01";
                        break;
                    default:
                        return "";
                        break;
                }
            }
            else if (this.state == DialogueState.TSUNDERE_01 && !this.audioSource.isPlaying)//0
            {
                switch (mode)
                {
                    case "TSUNDERE":
                        this.state = DialogueState.TSUNDERE_02;
                        this.dialogue.text = tsunTexts[2];
                        Debug.Log(tsunTexts[1]);
                        //this.audioSource.PlayOneShot(this.tsunVoices[1]);//1
                        this.audioSource.clip = this.tsunVoices[2];
                        this.audioSource.Play();
                        return "TSUNDERE_02";
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_02;
                        this.dialogue.text = dereTexts[2];
                        Debug.Log(dereTexts[1]);
                        //this.audioSource.PlayOneShot(this.dereVoices[1]);//1
                        this.audioSource.clip = this.dereVoices[2];
                        this.audioSource.Play();
                        return "TSUNDERE_02";
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
                        this.dialogue.text = tsunTexts[4];
                        Debug.Log(tsunTexts[4]);
                        //this.audioSource.PlayOneShot(this.tsunVoices[3]);//3
                        this.audioSource.clip = this.tsunVoices[4];
                        this.audioSource.Play();
                        return "TSUNDERE_04";
                        break;
                    case "DEREDERE":
                        this.state = DialogueState.TSUNDERE_04;
                        this.dialogue.text = dereTexts[4];
                        Debug.Log(dereTexts[4]);
                        //this.audioSource.PlayOneShot(this.dereVoices[3]);//3
                        this.audioSource.clip = this.dereVoices[4];
                        this.audioSource.Play();
                        return "TSUNDERE_04";
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
                return "FinishMenu";
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
                case DialogueState.Greeting:
                    return "Greeting";
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
                case DialogueState.Finish:
                    return "Finish";
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

