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

        if (stateInfo.IsName("Wait"))//Waitに変更した時
        {
            main.SetAnimationState("Wait");
        }
        if (stateInfo.IsName("Move"))//Moveに変更した時
        {
            main.SetAnimationState("Move");
        }
        if (stateInfo.IsName("Stanby"))//Stanbyに変更した時
        {
            main.SetAnimationState("Stanby");
        }
        if (stateInfo.IsName("First"))//Firstに変更した時
        {
            main.SetAnimationState("First");
        }
        if (stateInfo.IsName("WaitAnswer"))//WaitAnswerに変更した時
        {
            main.SetAnimationState("WaitAnswer");
        }
        if (stateInfo.IsName("Second"))//Secondに変更した時
        {
            main.SetAnimationState("Second");
        }
        if (stateInfo.IsName("Bye"))//Byeに変更した時
        {
            main.SetAnimationState("Bye");
        }
        if (stateInfo.IsName("ByeCheki"))//ByeChekiに変更した時
        {
            main.SetAnimationState("ByeCheki");
        }
        if (stateInfo.IsName("Back"))//Backに変更した時
        {
            main.SetAnimationState("Back");
        }
    }

    ////状態が終わる時(変わる直前)に実行
    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.Log($"Idle_B終了");
    //}
}
