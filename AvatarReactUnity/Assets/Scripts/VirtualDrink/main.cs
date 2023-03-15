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
    public bool cheki;//false: �`�F�L�Ȃ�, true: �`�F�L����
    public CameraShake shake;//�J������h�炷���߂̂���
    private AudioSource audioSource;//�����o�͂̂��߂Ɏg�p
    private AudioClip[] tsunVoices, dereVoices; //�����t�@�C��
    private string[] tsunTexts, dereTexts;//Dialogue�t�@�C��
    private ChangeDialogue changeDialogue;//change color and agent-name text in dialogue
    public string inputMessage;//��������SoundRecognizer_v2�ŏo�͂���郁�b�Z�[�W//state1 or state2

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

        //Resources���特���t�@�C���̒��o
        string folderName_TD = "TSUNDERE";//Voice/TSUNDERE���̃t�@�C�����擾
        UnityEngine.Object[] audioClips_TD = Resources.LoadAll("Voices/" + folderName_TD, typeof(AudioClip));
        string folderName_DD = "DEREDERE";//Voice/DEREDERE���̃t�@�C�����擾
        UnityEngine.Object[] audioClips_DD = Resources.LoadAll("Voices/" + folderName_DD, typeof(AudioClip));
        tsunVoices = new AudioClip[audioClips_TD.Length];//�t�@�C���T�C�Y�̐ݒ�
        dereVoices = new AudioClip[audioClips_DD.Length];//�t�@�C���T�C�Y�̐ݒ�
        for (int i = 0; i < audioClips_TD.Length; i++) this.tsunVoices[i] = Resources.Load("Voices/" + folderName_TD + "/" + audioClips_TD[i].name, typeof(AudioClip)) as AudioClip;
        for (int i = 0; i < audioClips_DD.Length; i++) this.dereVoices[i] = Resources.Load("Voices/" + folderName_DD + "/" + audioClips_DD[i].name, typeof(AudioClip)) as AudioClip;
        //TSUNDERE.txt�f�[�^�̓ǂݍ���
        TextAsset textassetTD = new TextAsset(); //�e�L�X�g�t�@�C���̃f�[�^���擾����C���X�^���X���쐬
        textassetTD = Resources.Load("Dialogues/TSUNDERE", typeof(TextAsset)) as TextAsset; //Resources�t�H���_����Ώۃe�L�X�g���擾
        int tsunSize = textassetTD.text.Split('\n').Length;
        tsunTexts = new string[tsunSize];
        string[] textMessageTD = textassetTD.text.Split('\n');
        for (int i = 0; i < tsunSize; i++) tsunTexts[i] = textMessageTD[i].Replace(" ", "\n");//���p�X�y�[�X�����s�ɕϊ�
                                                                                              //DEREDERE.txt�f�[�^�̓ǂݍ���
        TextAsset textassetDD = new TextAsset(); //�e�L�X�g�t�@�C���̃f�[�^���擾����C���X�^���X���쐬
        textassetDD = Resources.Load("Dialogues/DEREDERE", typeof(TextAsset)) as TextAsset; //Resources�t�H���_����Ώۃe�L�X�g���擾
        int dereSize = textassetDD.text.Split('\n').Length;
        dereTexts = new string[dereSize];
        string[] textMessageDD = textassetDD.text.Split('\n');
        for (int i = 0; i < dereSize; i++) dereTexts[i] = textMessageDD[i].Replace(" ", "\n");//���p�X�y�[�X�����s�ɕϊ�

        if (dialogue == null) dialogue = GameObject.Find("Canvas/Dialogue").gameObject.GetComponent<TextMeshProUGUI>();
        if (shake == null) shake = Camera.main.GetComponent<CameraShake>();

        inputMessage = "";
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.N) && agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))//�L�����N�^�[�̐U�镑���X�^�[�g�FN
        {
            agentAnimator.SetBool("mainBool",true);
        }
        if (Input.GetKey(KeyCode.A) && agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))//�A���P�[�g���烁�j���[�I��FA
        {
            pythonProgram py = this.GetComponent<pythonProgram>();
            py.ChangeBehaviourBasedOnQuestionnaire();
        }
        if (Input.GetKeyDown(KeyCode.I) && agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))//�蓮�Ń��j���[�؂�ւ��FI
        {
            if (agentNum == 0) agentNum = 1;
            else agentNum = 0;
            SetAgent(agentNum);
        }
        if (Input.GetKey(KeyCode.S))//�J������h�炷�FS
        {
            StartCoroutine(ShakeCamera());
        }
        if(agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && inputMessage == "state2")//���[�U����̉������͂��������Ƃ�
        {
            agentAnimator.SetBool("mainBool", true);
        }
        if (agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("WaitAnswer") && Input.GetKey(KeyCode.T))//�蓮�Ń��[�U����̉������͂�ݒ�FI
        {
            agentAnimator.SetBool("mainBool", true);
            Debug.Log("Ket T is clecked.");
        }

        

    }

    //Animation State���J�ڂ����Ƃ��Ɏ��s�����֐�
    public void SetAnimationState(string animationState)
    {
        switch (animationState)
        {
            case "Wait":
                agentAnimator.SetBool("mainBool", false);
                dialogue.text = "�X�^���o�C��...";
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
                agentAnimator.SetBool("cheki", cheki);//�`�F�L�̐ݒ�
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
                dialogue.text = "�X�^���o�C��...";
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

    //�J������h�炷�֐�
    IEnumerator ShakeCamera()
    {
        float time = 0.1f;
        shake.Shake(time, 0.03f);
        yield return new WaitForSecondsRealtime(time);
        Camera.main.transform.position = new Vector3(-2.45f, 1.3f, -10.85f);//�J�����������ʒu�ɖ߂�
        //Debug.Log("CameraPos: " + Camera.main.transform.position);
    }

    //�P�̃e�L�X�g�\���Ɖ����o�͂�����֐�
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
    //�Q�̘A�������e�L�X�g�\���Ɖ����o�͂�����֐�
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
        if (agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Second"))//�ЂƂO��mainBool��true->false�ɕω����Ȃ������̂ŁAtrigger���p
        {
            agentAnimator.SetTrigger("ChekiTrigger");
        }
    }
}

