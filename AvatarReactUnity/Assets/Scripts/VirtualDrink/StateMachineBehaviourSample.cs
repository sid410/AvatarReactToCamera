using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBehaviourSample : StateMachineBehaviour
{
    private main main;
    //状態が変わった時に実行
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (main == null) main = GameObject.Find("Main").GetComponent<main>();

        if (stateInfo.IsName("00_Stanby"))//00_Stanbyに変更した時
        {
            main.SetAnimationState("00_Stanby");
        }
        if (stateInfo.IsName("01_Notice"))//01_Noticeに変更した時
        {
            main.SetAnimationState("01_Notice");
        }
        if (stateInfo.IsName("02_MoveFrameOut1"))//02_MoveFrameOut1に変更した時
        {
            main.SetAnimationState("02_MoveFrameOut1");
        }
        if (stateInfo.IsName("03_MoveFrameIn1"))//03_MoveFrameIn1に変更した時
        {
            main.SetAnimationState("03_MoveFrameIn1");
        }
        if (stateInfo.IsName("04_Greeting1"))//04_Greeting1に変更した時
        {
            main.SetAnimationState("04_Greeting1");
        }
        if (stateInfo.IsName("05_Tsunderador1"))//05_Tsunderador1に変更した時
        {
            main.SetAnimationState("05_Tsunderador1");
        }
        if (stateInfo.IsName("05_Omakase1"))//05_Omakase1に変更した時
        {
            main.SetAnimationState("05_Omakase1");
        }
        if (stateInfo.IsName("06_WaitingForResponseTD"))//06_WaitingForResponseTDに変更した時
        {
            main.SetAnimationState("06_WaitingForResponseTD");
        }
        if (stateInfo.IsName("06_WaitingForResponseOM"))//06_WaitingForResponseOMに変更した時
        {
            main.SetAnimationState("06_WaitingForResponseOM");
        }
        if (stateInfo.IsName("07_Tsunderador2"))//07_Tsunderador2に変更した時
        {
            main.SetAnimationState("07_Tsunderador2");
        }
        if (stateInfo.IsName("idleAfterTD"))//bowに変更した時
        {
            main.SetAnimationState("idleAfterTD");
        }
        if (stateInfo.IsName("07_Omakase2"))//07_Omakase2に変更した時
        {
            main.SetAnimationState("07_Omakase2");
        }
        if (stateInfo.IsName("08_Greeting2"))//08_Greeting2に変更した時
        {
            main.SetAnimationState("08_Greeting2");
        }
        if (stateInfo.IsName("09_Greeting2CCOCheki"))//09_Greeting2CCOChekiに変更した時
        {
            main.SetAnimationState("09_Greeting2CCOCheki");
        }
    }

    ////状態が終わる時(変わる直前)に実行
    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.Log($"Idle_B終了");
    //}
}
