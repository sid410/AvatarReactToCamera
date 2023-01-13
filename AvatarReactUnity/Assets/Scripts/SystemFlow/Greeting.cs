using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Greeting : MonoBehaviour
{
    public TextMeshProUGUI dialogue;
    public string tsundereMode;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("������" + tsundereMode + "���n�߂܂��B���v�ł����H");
        dialogue.text = "������" + tsundereMode + "���n�߂܂��B���v�ł����H";
    }

    // Update is called once per frame
    void Update()
    {
        if (MotionTriger())
        {
            SceneManager.LoadScene("Tsunderadors");
        }
    }

    bool MotionTriger()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
        else
            return false;
    }
}
