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
        Debug.Log("今から" + tsundereMode + "を始めます。大丈夫ですか？");
        dialogue.text = "今から" + tsundereMode + "を始めます。大丈夫ですか？";
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
