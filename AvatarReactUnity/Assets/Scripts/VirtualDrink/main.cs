using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class main : MonoBehaviour
{
    public int agentNum; //0:Kaguya, 1:Manaka
    public GameObject agents;//AgentObject
    public GameObject agent; //Kaguya or Manaka
    public Animator[] agentAnimator; //agent's animator
    public TextMeshProUGUI dialogue, checkState, checkMode, checkAgent, checkCheki;
    public int mode; //0: Tsunderador, 1: Omakase soda
    public bool cheki;//false: チェキなし, true: チェキあり
    private CameraShake shake;//カメラを揺らすためのもの
    private AudioSource audioSource;//音声出力のために使用
    private AudioClip[] tsunVoices, dereVoices; //音声ファイル
    private string[] tsunTexts, dereTexts;//Dialogueファイル
    private ChangeDialogue changeDialogue;//change color and agent-name text in dialogue
    public string inputMessage;//音声入力SoundRecognizer_v2で出力されるメッセージ//state1 or state2

    private Vector3 posStanby, posGreeting, posBehaving;
    private Vector3 rotStanby, rotGreeting, rotBehaving;
    private bool posRotState, posRotState2; //agentの位置姿勢変更に使用（trueの時に変更、falseで無視）
    private AutoBlinkVRM[] autoBlinkVRM;//表情を動かすためのクラス
    private Fade panel;

    public MessageSender messageSender;

    public string CurrentState;

    void Start()
    {
        if (agents == null) agents = GameObject.Find("Agents");
        agentAnimator = new Animator[agents.transform.childCount];
        autoBlinkVRM = new AutoBlinkVRM[agents.transform.childCount];
        for (int i=0; i < agents.transform.childCount; i++)
        {
            //アニメーターの登録
            agentAnimator[i] = agents.transform.GetChild(i).gameObject.GetComponent<Animator>();

            //モードの登録
            if (mode == 0) agentAnimator[i].SetBool("tsundere", true);
            else if (mode == 1) agentAnimator[i].SetBool("tsundere", false);
            else Debug.Log("Error: please set mode 0 or 1.");

            //チェキの登録
            agentAnimator[i].SetBool("cheki", cheki);

            //表情を動かすためのクラスを継承+表情のリセット
            autoBlinkVRM[i] = agents.transform.GetChild(i).gameObject.GetComponent<AutoBlinkVRM>();
        }
        if (changeDialogue == null) changeDialogue = gameObject.GetComponent<ChangeDialogue>();
        if (audioSource == null) audioSource = gameObject.GetComponent<AudioSource>();
        if (agentNum == 0)
        {
            changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
            agent = agents.transform.GetChild(agentNum).gameObject;
        }
        else if (agentNum == 1)
        {
            changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
            agent = agents.transform.GetChild(agentNum).gameObject;
        }
        else
        {
            Debug.Log("Error: please set agentNum to 0-1.");
        }


        //エージェントの初期位置姿勢の設定
        for(int i =0; i<2; i++)
        {
            agents.transform.GetChild(i).gameObject.transform.localPosition = GameObject.Find("Stanby").transform.GetChild(i).transform.localPosition;
            agents.transform.GetChild(i).gameObject.transform.localRotation = GameObject.Find("Stanby").transform.GetChild(i).transform.localRotation;
        }
        posStanby = GameObject.Find("Stanby").transform.GetChild(agentNum).transform.localPosition;
        rotStanby = GameObject.Find("Stanby").transform.GetChild(agentNum).transform.localRotation.eulerAngles;
        posGreeting = GameObject.Find("Greeting").transform.GetChild(agentNum).transform.localPosition;
        rotGreeting = GameObject.Find("Greeting").transform.GetChild(agentNum).transform.localRotation.eulerAngles;
        posBehaving = GameObject.Find("Behaving").transform.GetChild(agentNum).transform.localPosition;
        rotBehaving = GameObject.Find("Behaving").transform.GetChild(agentNum).transform.localRotation.eulerAngles;


        //Resourcesから音声ファイルの抽出
        SetAgentVoice(agentNum);
        SetDialogue(agentNum);


        if (dialogue == null) dialogue = GameObject.Find("Canvas/Dialogue").gameObject.GetComponent<TextMeshProUGUI>();//メッセージボックスのテキスト情報を変更するためのクラスを継承
        if (checkState == null) checkState = GameObject.Find("Canvas/State").gameObject.GetComponent<TextMeshProUGUI>();
        if (checkMode == null) checkMode = GameObject.Find("Canvas/Mode").gameObject.GetComponent<TextMeshProUGUI>();
        if (checkAgent == null) checkAgent = GameObject.Find("Canvas/Agent").gameObject.GetComponent<TextMeshProUGUI>();
        if (checkCheki == null) checkCheki = GameObject.Find("Canvas/Cheki").gameObject.GetComponent<TextMeshProUGUI>();
        if (shake == null) shake = Camera.main.GetComponent<CameraShake>();//カメラを揺らすためのクラスを継承

        inputMessage = "";
        

        if (panel == null) panel = GameObject.Find("Canvas/Panel").gameObject.GetComponent<Fade>();
    }

    

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("agentPos: " + agent.transform.localPosition + ", agentRot: " + agent.transform.localRotation.eulerAngles);
        if (Input.GetKey(KeyCode.N) && agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("00_Stanby"))//キャラクターの振る舞いスタート：N
        {
            pythonProgram py = this.GetComponent<pythonProgram>();
            py.ChangeBehaviourBasedOnQuestionnaire();
            inputMessage = "";
            
        }
        if (Input.GetKey(KeyCode.A) && agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("00_Stanby"))//アンケートからメニュー選定：A
        {
            pythonProgram py = this.GetComponent<pythonProgram>();
            py.ChangeBehaviourBasedOnQuestionnaire();
        }
        if (Input.GetKeyDown(KeyCode.I) && agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("00_Stanby"))//手動でメニュー切り替え：I
        {
            if (mode==0)
            {
                mode = 1;// change to Omakase soda
                agentAnimator[agentNum].SetBool("tsundere", false);
            }
            else
            {
                mode = 0;//change to Tsunderador
                agentAnimator[agentNum].SetBool("tsundere", true);
            }
            SetAgent(agentNum);
        }
        if (Input.GetKey(KeyCode.S))//カメラを揺らす：S
        {
            StartCoroutine(ShakeCamera());
        }

        //Parameter: responseの処理
        if(agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("06_WaitingForResponseTD")|| agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("06_WaitingForResponseOM"))
        {
            if(inputMessage == "answer")//ユーザからの音声入力があったとき
            {
                //agent.transform.localPosition = posBehaving;
                //agent.transform.localRotation = Quaternion.Euler(rotBehaving);
                agentAnimator[agentNum].SetBool("response", true);
                Debug.Log("response: true");
            }
            if (Input.GetKey(KeyCode.T))//手動でユーザからの音声入力を設定：T
            {
                //agent.transform.localPosition = posBehaving;
                //agent.transform.localRotation = Quaternion.Euler(rotBehaving);
                agentAnimator[agentNum].SetBool("response", true);
                Debug.Log("Ket T is clecked.");
            }
           

        }
       


        //エージェントの位置姿勢設定
        if (agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("00_Stanby") && posRotState)//State: Stanby開始時にの位置姿勢変更
        {
            ////agent.transform.localPosition = posStanby;
            ////agent.transform.localRotation = Quaternion.Euler(rotStanby);
            posRotState = false;
        }
        
        else if (agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("04_Greeting1") && posRotState)//State: 04_Greeting1開始時にの位置姿勢変更
        {
            //agent.transform.localPosition = posGreeting;
            //agent.transform.localRotation = Quaternion.Euler(rotGreeting);
            //Debug.Log("Greeting AgentPos: " + agent.transform.position);
            posRotState = false;
        }
        else if (agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("05_Tsunderador1") && posRotState)//State: 05_Tsunderador1開始時にの位置姿勢変更
        {
            agent.transform.localPosition = posBehaving;
            agent.transform.localRotation = Quaternion.Euler(rotBehaving);
            posRotState = false;
        }
        else if (agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("05_Omakase1") && posRotState)//State: 05_Omakase1開始時にの位置姿勢変更
        {
            agent.transform.localPosition = posBehaving;
            agent.transform.localRotation = Quaternion.Euler(rotBehaving);
            posRotState = false;
        }
        else if (agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("03_MoveFrameIn1"))//State: 03_MoveFrameIn1開始時にの位置姿勢変更
        {
            if (posRotState2)
            {
                agent.transform.localPosition = posGreeting;
                agent.transform.localRotation = Quaternion.Euler(rotGreeting);
                agentAnimator[agentNum].SetBool("walking", false);
                posRotState2 = false;
            }

        }
        else if (agentAnimator[agentNum].GetCurrentAnimatorStateInfo(0).IsName("02_MoveFrameOut1"))//State: 02_MoveFrameOut1開始時にの位置姿勢変更
        {
            if (agentNum == 0)
            {
                if (agent.transform.localPosition.x < -4.5f) agentAnimator[agentNum].SetBool("walking", false);
                else WalkingToLeft();
            }
            else
            {
                if (agent.transform.localPosition.x < -5.5f)agentAnimator[agentNum].SetBool("walking", false);
                else WalkingToLeft();
            }
            
        }
        

    }

    public void SetAgentVoice(int agentNUM)
    {
        string agentName = "";
        if (agentNUM == 0)
        {
            agentName = "Kaguya";
        }else if (agentNUM == 1)
        {
            agentName = "Manaka";
        }
        //Resourcesから音声ファイルの抽出
        string folderName_TD = "TSUNDERE";//Voice/TSUNDERE内のファイルを取得
        UnityEngine.Object[] audioClips_TD = Resources.LoadAll("Voices/" + agentName + "/" + folderName_TD, typeof(AudioClip));
        string folderName_DD = "DEREDERE";//Voice/DEREDERE内のファイルを取得
        UnityEngine.Object[] audioClips_DD = Resources.LoadAll("Voices/" + agentName + "/" + folderName_DD, typeof(AudioClip));
        tsunVoices = new AudioClip[audioClips_TD.Length];//ファイルサイズの設定
        dereVoices = new AudioClip[audioClips_DD.Length];//ファイルサイズの設定
        for (int i = 0; i < audioClips_TD.Length; i++) this.tsunVoices[i] = Resources.Load("Voices/" + agentName + "/" + folderName_TD + "/" + audioClips_TD[i].name, typeof(AudioClip)) as AudioClip;
        for (int i = 0; i < audioClips_DD.Length; i++) this.dereVoices[i] = Resources.Load("Voices/" + agentName + "/" + folderName_DD + "/" + audioClips_DD[i].name, typeof(AudioClip)) as AudioClip;
    }

    public void SetDialogue(int agentNUM)
    {
        //TSUNDERE.txtデータの読み込み
        TextAsset textassetTD = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        TextAsset textassetDD = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        if (agentNUM == 0)
        {
            textassetTD = Resources.Load("Dialogues/Kaguya/TSUNDERE", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
            textassetDD = Resources.Load("Dialogues/Kaguya/DEREDERE", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
        }
        else
        {
            textassetTD = Resources.Load("Dialogues/Manaka/TSUNDERE", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
            textassetDD = Resources.Load("Dialogues/Manaka/DEREDERE", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
        }
       
        int tsunSize = textassetTD.text.Split('\n').Length;
        tsunTexts = new string[tsunSize];
        string[] textMessageTD = textassetTD.text.Split('\n');
        for (int i = 0; i < tsunSize; i++) tsunTexts[i] = textMessageTD[i].Replace(" ", "\n");//半角スペースを改行に変換
                                                                                              //DEREDERE.txtデータの読み込み
        
        int dereSize = textassetDD.text.Split('\n').Length;
        dereTexts = new string[dereSize];
        string[] textMessageDD = textassetDD.text.Split('\n');
        for (int i = 0; i < dereSize; i++) dereTexts[i] = textMessageDD[i].Replace(" ", "\n");//半角スペースを改行に変換
    } 

    //Animation Stateが遷移したときに実行される関数（なぜかこの関数の中ではagentの位置姿勢の変更ができない。。。代わりにupdate関数内で変更）
    public void SetAnimationState(string animationState)
    {
        switch (animationState)
        {
            case "00_Stanby":
                //各パラメータの初期化
                posRotState = true;
                agentAnimator[agentNum].SetBool("start", false);
                agentAnimator[agentNum].SetBool("response", false);
                agentAnimator[agentNum].SetBool("waitingSound", false);
                agentAnimator[agentNum].SetBool("walking", true);
                panel.FadeIn();
                agent.transform.localPosition = posStanby;
                agent.transform.localRotation = Quaternion.Euler(rotStanby);
                autoBlinkVRM[agentNum].FaceReset();
                dialogue.text = "スタンバイ中...";
                checkState.text = "State: 00_Stanby";
                Debug.Log("State: 00_Stanby");
                break;
            case "02_MoveFrameOut1":
                agentAnimator[agentNum].SetBool("start", false);
                agentAnimator[agentNum].SetBool("walking", true);
                autoBlinkVRM[agentNum].Smile();
                checkState.text = "State: 02_MoveFrameOut1";
               Debug.Log(checkState.text);
                break;
            case "03_MoveFrameIn1":
                posRotState2 = true;
                checkState.text = "State: 03_MoveFrameIn1";
                autoBlinkVRM[agentNum].Smile();
                Debug.Log(checkState.text);
                //panel.FadeOut();
                panel.FadeIn();
                break;
            case "04_Greeting1":
                posRotState = true;
                if (mode == 0) agentAnimator[agentNum].SetBool("tsundere", true);
                else agentAnimator[agentNum].SetBool("tsundere", false);
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                checkState.text = "State: 04_Greeting1";
                Debug.Log(checkState.text);
                StartCoroutine(playAudio(0));
                break;
            case "05_Tsunderador1":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Angry();
                checkState.text = "State: 05_Tsunderador1";
                Debug.Log(checkState.text);
                StartCoroutine(ContinuousPlay(1,3));
                break;
            case "05_Omakase1":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                checkState.text = "State: 05_Omakase1";
                Debug.Log(checkState.text);
                StartCoroutine(ContinuousPlay(1,2));
                break;
            case "06_WaitingForResponseTD":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Angry();
                checkState.text = "State: 06_WaitingForResponseTD";
                Debug.Log(checkState.text);
                break;
            case "06_WaitingForResponseOM":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                checkState.text = "State: 06_WaitingForResponseOM";
                Debug.Log(checkState.text);
                break;
            case "07_Tsunderador2":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                StartCoroutine(ContinuousPlay(4,2));
                checkState.text = "State: 07_Tsunderador2";
                Debug.Log(checkState.text);
                break;
            case "07_Omakase2":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                StartCoroutine(ContinuousPlay(3,4));
                checkState.text = "State: 07_Omakase2";
                Debug.Log(checkState.text);
                break;
            case "idleAfterTD":
                autoBlinkVRM[agentNum].Smile();
                checkState.text = "State: idleAfterTD";
                Debug.Log(checkState.text);
                break;
            case "08_Greeting2":
                posRotState = true;
                if (cheki) agentAnimator[agentNum].SetBool("cheki", true);
                else agentAnimator[agentNum].SetBool("cheki", false);
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                if (mode==0) StartCoroutine(playAudio(6));
                else StartCoroutine(playAudio(7));
                checkState.text = "State: 08_Greeting2";
               Debug.Log(checkState.text);
                break;
            case "09_Greeting2CCOCheki":
                posRotState = true;
                agentAnimator[agentNum].SetBool("waitingSound", false);
                autoBlinkVRM[agentNum].Smile();
                checkState.text = "State: 09_Greeting2CCOCheki";
                Debug.Log(checkState.text);
                if (mode == 0) StartCoroutine(playAudio(7));
                else StartCoroutine(playAudio(8));
                StartCoroutine(PlayCCOcheki());
                break;
            default:
                break;
        }
        CurrentState = animationState;
    }
    //エージェントを設定する関数（現状（3/15）：エージェントの種類＝ツンデレorデレデレ）
    public void SetAgent(int agentNUM)
    {
        agentNum = agentNUM;
        switch (agentNum)
        {
            case 0://Kaguya
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                Debug.Log("Change to Kaguya.");
                break;
            case 1://Manaka
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                Debug.Log("Change to Manaka.");
                break;
            default:
                break;
        }
        if (agents == null) agents = GameObject.Find("Agents");
        agent = agents.transform.GetChild(agentNum).gameObject;
        autoBlinkVRM[agentNum].FaceReset();
        //エージェントの位置姿勢設定
        posStanby = GameObject.Find("Stanby").transform.GetChild(agentNum).transform.localPosition;
        rotStanby = GameObject.Find("Stanby").transform.GetChild(agentNum).transform.localRotation.eulerAngles;
        posGreeting = GameObject.Find("Greeting").transform.GetChild(agentNum).transform.localPosition;
        rotGreeting = GameObject.Find("Greeting").transform.GetChild(agentNum).transform.localRotation.eulerAngles;
        posBehaving = GameObject.Find("Behaving").transform.GetChild(agentNum).transform.localPosition;
        rotBehaving = GameObject.Find("Behaving").transform.GetChild(agentNum).transform.localRotation.eulerAngles;
        //animation
        agentAnimator[agentNum].SetBool("response", false);
        agentAnimator[agentNum].SetBool("waitingSound", false);
        agentAnimator[agentNum].SetBool("walking", true);
        agentAnimator[agentNum].SetBool("start", true);
    }

    public void Daipan()
    {
        StartCoroutine(ShakeCamera());
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

    //１つのテキスト表示と音声出力させる関数
    IEnumerator playAudio(int audioClipNum)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        switch (mode)
        {
            case 0:
                audioSource.clip = tsunVoices[audioClipNum];
                audioSource.Play();
                dialogue.text = tsunTexts[audioClipNum];
                break;
            case 1:
                audioSource.clip = dereVoices[audioClipNum];
                audioSource.Play();
                dialogue.text = dereTexts[audioClipNum];
                break;
            default:
                break;
        }
        yield return new WaitForSecondsRealtime(audioSource.clip.length+0.3f);//音声ファイルが再生されている時間待機
        agentAnimator[agentNum].SetBool("waitingSound", true);
    }
    //２つの連続したテキスト表示と音声出力させる関数
    IEnumerator ContinuousPlay(int audioClipNum, int size)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        switch (mode)
        {
            case 0:
                for (int i = audioClipNum; i < audioClipNum + size; i++)
                {
                    audioSource.clip = tsunVoices[i];
                    audioSource.Play();
                    dialogue.text = tsunTexts[i];
                    yield return new WaitForSecondsRealtime(audioSource.clip.length);//音声ファイルが再生されている時間待機
                }
                agentAnimator[agentNum].SetBool("waitingSound", true);
                break;
            case 1:
                for (int i = audioClipNum; i < audioClipNum + size; i++)
                {
                    audioSource.clip = dereVoices[i];
                    audioSource.Play();
                    dialogue.text = dereTexts[i];
                    yield return new WaitForSecondsRealtime(audioSource.clip.length);//音声ファイルが再生されている時間待機
                }
                agentAnimator[agentNum].SetBool("waitingSound", true);
                break;
            default:
                break;
        }
       
    }
    IEnumerator PlayCCOcheki()
    {
        yield return new WaitForSecondsRealtime(15f);
        messageSender.SendChekiStatus();
    }

    public void WalkingToLeft()
    {
        if (agentNum == 0)
        {
            agent.transform.Translate(0, 0, 0.006f);
        }
        else if (agentNum == 1)
        {
            agent.transform.Translate(0, 0, 0.006f);
        }
        
    }
    public void WalkingFromLeft()
    {
        agent.transform.Translate(0, 0, 0.006f);
    }
}

