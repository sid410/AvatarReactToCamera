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
    public Vector3 stanbyPostion;

    GameObject udpSender;
    UDPSender udpSenderScript;

    private Animator animator;
    private AudioSource[] sources;
    private DialogueManager dialogueManager;
    private bool isReview;

    void Start()
    {
        animator = GetComponent<Animator>();
        inputMessage = "";

        state = new StateManager("Start");
        isFinish = true;
        isReview = false;

        //sources = gameObject.GetComponents<AudioSource>();
        dialogueManager = new DialogueManager(dialogue, gameObject.GetComponents<AudioSource>());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("dialogueManager State :" + dialogueManager.getState());
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
            state.nextState();
            animator.SetBool("isConfirm", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !isReview)
        {
            Debug.Log("Wait for going to review");
            isReview = true;
            StartCoroutine(dialogueManager.StartReview(3.0f));
        }
        else if (dialogueManager.getState() == "WaitAnswer")
        {
            Debug.Log("Wait for answer");
            StartCoroutine(dialogueManager.WaitFeedback(1.5f));
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("OutSide") && dialogueManager.getState() == "Finish")
        {
            state.nextState();
            animator.SetBool("isMoved", true);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("OutSide"))
        {
            Debug.Log("OutSide");
            animator.SetBool("isMoved", false);
            animator.SetBool("isOut", true);
            this.gameObject.transform.position = stanbyPostion;
        }

    }

    bool MotionTriger()
    {
        //if (state.getState() == "Start" && Input.GetKey(KeyCode.LeftShift)) return true;
        //if (state.getState() == "TsunderadorFirst" && Input.GetKey(KeyCode.LeftControl)) return true;
        //else return false;

        if (state.getState() == "Start" && (inputMessage == "state1" || Input.GetKey(KeyCode.LeftShift))) return true;
        if (state.getState() == "TsunderadorFirst" && (inputMessage == "state2" || Input.GetKey(KeyCode.LeftControl))) return true;
        else return false;
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
            AskThoughts,
            WaitAnswer,
            Response,
            Finish,
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
            else if (mode == "AfterTalk")
            {
                this.state = DialogueState.AskThoughts;
                this.dialogue.text = "どうだった？上手にツンデレーダーできてた？";
                Debug.Log("どうだった？上手にツンデレーダーできてた？");
                this.sources[4].Play();
            }
            else if (mode == "Response")
            {
                this.state = DialogueState.Response;
                this.dialogue.text = "あっそ！その程度なんてまだまだね！次はもっと楽しませてあげるんだから！";
                Debug.Log("あっそ！その程度なんてまだまだね！次はもっと楽しませてあげるんだから！");
                this.sources[5].Play();
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
                return "FinishMenu";
            }
            else if (this.state == DialogueState.AskThoughts && !this.sources[4].isPlaying)
            {
                this.state = DialogueState.WaitAnswer;
                activateMike();
                Debug.Log("Stanby for the answer");
                return "WaitAnswer";
            }
            else if (this.state == DialogueState.Response && !this.sources[5].isPlaying)
            {
                this.state = DialogueState.Finish;
                this.dialogue.text = "ありがとうございました！";
                Debug.Log("ありがとうございました！" + this.getState());
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
            else if (this.state == DialogueState.AskThoughts) return "AskThoughts";
            else if (this.state == DialogueState.WaitAnswer) return "WaitAnswer";
            else if (this.state == DialogueState.Response) return "Response";
            else if (this.state == DialogueState.Finish) return "Finish";
            else return "cannot get state";
        }

        // 終了後レビュー開始前待ち用コルーチン
        public IEnumerator StartReview(float delay_time)
        {
            yield return new WaitForSeconds(delay_time);

            activateAudio("AfterTalk");
        }

        // 感想入力待ち用コルーチン
        public IEnumerator WaitFeedback(float delay_time)
        {
            yield return new WaitForSeconds(delay_time);

            deactivateMike();
            activateAudio("Response");
        }

        private void activateMike()
        {
            // 起動時のみTrue
            // それ以外はFalse
            Debug.Log("Activate Mike");
        }

        private void deactivateMike()
        {
            // 終了時のみTrue
            // それ以外はFalse
            Debug.Log("Deactivate Mike");
        }
    }
}
