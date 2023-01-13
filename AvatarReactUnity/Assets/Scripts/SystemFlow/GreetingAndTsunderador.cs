using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GreetingAndTsunderador : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        XMLController xml_contoller = new XMLController();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stanby") && MotionTriger())
        {
            animator.SetBool("isStart", true);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("StartGreeting") && MotionTriger())
        {
            animator.SetBool("isStart", false);
            animator.SetBool("isAreYouOK", true);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("TsunderadorStep1") && MotionTriger())
        {
            animator.SetBool("isAreYouOK", false);
            animator.SetBool("isDoYouUnderstand", true);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("TsunderadorStep2") && MotionTriger())
        {
            animator.SetBool("isDoYouUnderstand", false);
            animator.SetBool("isDoYouTakeCheki", true);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("MoveToChekiPoint") && MotionTriger())
        {
            animator.SetBool("isDoYouTakeCheki", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("FinishScene1"))
        {
            Debug.Log("Change Scene");
            SceneManager.LoadScene("TakeCheki");
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

public class XMLController
{
    void get_xml()
    {
        //Accept XML File
    }
}
