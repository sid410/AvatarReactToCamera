using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeDialogue : MonoBehaviour
{
    public Color dialogueColorTD, dialogueColorDD;
    public string agentName = "‚©‚®‚â";
    public TextMeshProUGUI nameText;
    public Image outlineTop, outlineBottom, nameBackground;
    // Start is called before the first frame update
    void Start()
    {
        dialogueColorTD = new Color(0.49f, 0.76f, 1.0f, 1.0f);
        dialogueColorDD = new Color(0.98f, 0.46f, 0.45f, 1.0f);
        agentName = "‚©‚®‚â";
        if (nameBackground == null) nameBackground = GameObject.Find("Canvas/NameBackground").GetComponent<Image>();
        if (nameText == null) nameText = GameObject.Find("Canvas/NameBackground/Nametext").GetComponent<TextMeshProUGUI>();
        if (outlineTop == null) outlineTop = GameObject.Find("Canvas/OutlineTop").GetComponent<Image>();
        if (outlineBottom == null) outlineBottom = GameObject.Find("Canvas/OutlineBottom").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDialogue(int agentNum)
    {
        switch (agentNum)
        {
            case 0:
                nameText.text = "‚©‚®‚â";
                nameBackground.color = dialogueColorTD;
                outlineTop.color = dialogueColorTD;
                outlineBottom.color = dialogueColorTD;
                break;
            case 1:
                nameText.text = "‚Ü‚È‚©";
                nameBackground.color = dialogueColorDD;
                outlineTop.color = dialogueColorDD;
                outlineBottom.color = dialogueColorDD;
                break;
            default:
                break;
        }
    }
}
