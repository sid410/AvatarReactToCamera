using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeCheki : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && MotionTriger())
        {
            animator.SetBool("isSetPose", true);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeCheki") && MotionTriger())
        {
            animator.SetBool("isSetPose", false);
            animator.SetBool("isFinishPose", true);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("FinishCheki"))
        {
            animator.SetBool("isFinishPose", false);
            Debug.Log("Change Scene");
            SceneManager.LoadScene("TsunderadorAndGreeting");
        }
    }

    bool MotionTriger()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return true;
        }
        else
            return false;
    }
}
