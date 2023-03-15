using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBehaviourSample : StateMachineBehaviour
{
    private main main;
    //��Ԃ��ς�������Ɏ��s
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (main == null) main = GameObject.Find("Main").GetComponent<main>();

        if (stateInfo.IsName("Wait"))//Wait�ɕύX������
        {
            main.SetAnimationState("Wait");
        }
        if (stateInfo.IsName("Move"))//Move�ɕύX������
        {
            main.SetAnimationState("Move");
        }
        if (stateInfo.IsName("Stanby"))//Stanby�ɕύX������
        {
            main.SetAnimationState("Stanby");
        }
        if (stateInfo.IsName("First"))//First�ɕύX������
        {
            main.SetAnimationState("First");
        }
        if (stateInfo.IsName("WaitAnswer"))//WaitAnswer�ɕύX������
        {
            main.SetAnimationState("WaitAnswer");
        }
        if (stateInfo.IsName("Second"))//Second�ɕύX������
        {
            main.SetAnimationState("Second");
        }
        if (stateInfo.IsName("Bye"))//Bye�ɕύX������
        {
            main.SetAnimationState("Bye");
        }
        if (stateInfo.IsName("ByeCheki"))//ByeCheki�ɕύX������
        {
            main.SetAnimationState("ByeCheki");
        }
        if (stateInfo.IsName("Back"))//Back�ɕύX������
        {
            main.SetAnimationState("Back");
        }
    }

    ////��Ԃ��I��鎞(�ς�钼�O)�Ɏ��s
    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.Log($"Idle_B�I��");
    //}
}
