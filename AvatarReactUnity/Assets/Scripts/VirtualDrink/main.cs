using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class main : MonoBehaviour
{
    public int agentNum; //0:Kaguya, 1:Manaka
    public GameObject agents;//AgentObject
    private GameObject agent; //Kaguya or Manaka
    private Animator agentAnimator; //agent's animator
    public TextMeshProUGUI dialogue, checkText;
    public string mode;
    public bool cheki;//false: チェキなし, true: チェキあり
    public CameraShake shake;//カメラを揺らすためのもの
    private AudioSource audioSource;//音声出力のために使用
    private AudioClip[] tsunVoices, dereVoices; //音声ファイル
    private string[] tsunTexts, dereTexts;//Dialogueファイル
    private ChangeDialogue changeDialogue;//change color and agent-name text in dialogue
    public string inputMessage;//音声入力SoundRecognizer_v2で出力されるメッセージ//state1 or state2

    public Vector3 posStanby;
    public Vector3 rotStanby;

    void Start()
    {
        if (agents == null) agents = GameObject.Find("Agent");
        if (changeDialogue == null) changeDialogue = gameObject.GetComponent<ChangeDialogue>();
        if (audioSource == null) audioSource = gameObject.GetComponent<AudioSource>();
        if (agentNum == 0)
        {
            mode = "TSUNDERE";
            changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
        }
        else if (agentNum == 1)
        {
            mode = "DEREDERE";
            changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
        }
        agent = agents.transform.GetChild(agentNum).gameObject;
        agentAnimator = agent.GetComponent<Animator>();
        agentAnimator.SetBool("mainBool", false);
        agentAnimator.SetBool("cheki", cheki);

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

        if (dialogue == null) dialogue = GameObject.Find("Canvas/Dialogue").gameObject.GetComponent<TextMeshProUGUI>();
        if (shake == null) shake = Camera.main.GetComponent<CameraShake>();

        inputMessage = "";
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.N) && agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))//キャラクターの振る舞いスタート：N
        {
            agentAnimator.SetBool("mainBool",true);
        }
        if (Input.GetKey(KeyCode.A) && agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))//アンケートからメニュー選定：A
        {
            pythonProgram py = this.GetComponent<pythonProgram>();
            py.ChangeBehaviourBasedOnQuestionnaire();
        }
        if (Input.GetKeyDown(KeyCode.I) && agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))//手動でメニュー切り替え：I
        {
            if (agentNum == 0) agentNum = 1;
            else agentNum = 0;
            SetAgent(agentNum);
        }
        if (Input.GetKey(KeyCode.S))//カメラを揺らす：S
        {
            StartCoroutine(ShakeCamera());
        }
        if(agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && inputMessage == "state2")//ユーザからの音声入力があったとき
        {
            agentAnimator.SetBool("mainBool", true);
        }
        if (agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && Input.GetKey(KeyCode.T))//手動でユーザからの音声入力を設定：I
        {
            agentAnimator.SetBool("mainBool", true);
            Debug.Log("Ket T is clecked.");
        }

        

    }

    //Animation Stateが遷移したときに実行される関数
    public void SetAnimationState(string animationState)
    {
        switch (animationState)
        {
            case "Wait":
                agentAnimator.SetBool("mainBool", false);
                dialogue.text = "スタンバイ中...";
                Debug.Log("State: Wait");
                break;
            case "Move":
                agentAnimator.SetBool("mainBool", false);
                Debug.Log("State: Move");
                break;
            case "Stanby":
                agent.transform.position = posStanby;
                agent.transform.eulerAngles = rotStanby;
                agentAnimator.SetBool("mainBool", false);
                Debug.Log("State: Stanby");
                playAudio(0);
                break;
            case "First":
                agentAnimator.SetBool("mainBool", false);
                Debug.Log("State: First");
                StartCoroutine(ContinuousPlay(1));
                break;
            case "WaitAnswer":
                agentAnimator.SetBool("mainBool", false);
                Debug.Log("State: WaitAnswer");
                agentAnimator.SetBool("cheki", cheki);//チェキの設定
                break;
            case "Second":
                agentAnimator.SetBool("mainBool", false);
                StartCoroutine(ContinuousPlay(3));
                Debug.Log("State: Second");
                break;
            case "Bye":
                agentAnimator.SetBool("mainBool", false);
                playAudio(6);
                Debug.Log("State: Bye");
                break;
            case "ByeCheki":
                agentAnimator.SetBool("mainBool", false);
                Debug.Log("State: ByeCheki");
                playAudio(5);
                break;
            case "Back":
                agentAnimator.SetBool("mainBool", false);
                Debug.Log("State: Back");
                dialogue.text = "スタンバイ中...";
                break;
            default:
                break;
        }
    }
    public void SetAgent(int agentNUM)
    {
        agentNum = agentNUM;
        switch (agentNum)
        {
            case 0://Kaguya
                mode = "TSUNDERE";
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                break;
            case 1://Manaka
                mode = "DEREDERE";
                changeDialogue.SetDialogue(agentNum);//change color and agent-name text in dialogue
                break;
            default:
                break;
        }
        if (agents == null) agents = GameObject.Find("Agent");
        agent = agents.transform.GetChild(agentNum).gameObject;
        agentAnimator = agent.GetComponent<Animator>();

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
    private void playAudio(int audioClipNum)
    {
        switch (mode)
        {
            case "TSUNDERE":
                audioSource.clip = tsunVoices[audioClipNum];
                audioSource.Play();
                dialogue.text = tsunTexts[audioClipNum];
                break;
            case "DEREDERE":
                audioSource.clip = dereVoices[audioClipNum];
                audioSource.Play();
                dialogue.text = dereTexts[audioClipNum];
                break;
            default:
                break;
        }
    }
    //２つの連続したテキスト表示と音声出力させる関数
    IEnumerator ContinuousPlay(int audioClipNum)
    {
        switch (mode)
        {
            case "TSUNDERE":
                audioSource.clip = tsunVoices[audioClipNum];
                audioSource.Play();
                dialogue.text = tsunTexts[audioClipNum];
                break;
            case "DEREDERE":
                audioSource.clip = dereVoices[audioClipNum];
                audioSource.Play();
                dialogue.text = dereTexts[audioClipNum];
                break;
            default:
                break;
        }
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        switch (mode)
        {
            case "TSUNDERE":
                audioSource.clip = tsunVoices[audioClipNum+1];
                audioSource.Play();
                dialogue.text = tsunTexts[audioClipNum+1];
                break;
            case "DEREDERE":
                audioSource.clip = dereVoices[audioClipNum+1];
                audioSource.Play();
                dialogue.text = dereTexts[audioClipNum+1];
                break;
            default:
                break;
        }
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        agentAnimator.SetBool("mainBool", true);
        if (agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Second"))//ひとつ前でmainBoolをtrue->falseに変化しなかったので、triggerを代用
        {
            agentAnimator.SetTrigger("ChekiTrigger");
        }
    }
}

