using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class AutoBlinkVRM : MonoBehaviour
{

    [SerializeField]
    float blinkTime = 0.1f;//–Ϊ‚παÒ‚Α‚Δ‚Ά‚ιΤ
    [SerializeField]
    float blinkInterval = 1.0f;//u‚«‚Ζu‚«‚ΜΤ‚ΜΤ

    bool blinkEnabled = true;
    bool blinking = false;//true: u‚«’†,false: u‚«‚µ‚Δ‚Ά‚Θ‚Ά‚Ζ‚«
    BlendShapePreset currentFace;
    VRMBlendShapeProxy proxy;
    float deltaTime;

    void Start()
    {
        proxy = GetComponent<VRMBlendShapeProxy>();

        //ƒfƒtƒHƒ‹ƒg‚Μ•\ξ‚πƒZƒbƒg
        currentFace = BlendShapePreset.Neutral;
        proxy.AccumulateValue(currentFace, 1);
        Debug.Log("Test: Start");

        deltaTime = 0.0f;
    }


    void FixedUpdate()
    {
        //AutoBlink
        //‚±‚±‚©‚η
        if (blinking) {//u‚«‚µ‚Δ‚Ά‚ι
            if (deltaTime > blinkTime)
            {
                deltaTime = 0.0f;
                blinking = false;
                //Debug.Log("She Blinked!");
            }
            else
            {
                proxy.AccumulateValue(BlendShapePreset.Blink, 1);
            }
        }
        else//u‚«‚µ‚Δ‚Ά‚Θ‚Ά
        {
            if (deltaTime > blinkInterval)
            {
                deltaTime = 0.0f;
                blinking = true;
                blinkInterval = Random.Range(2, 6);//2-5•b‚ΜΤ‚Εƒ‰ƒ“ƒ_ƒ€‚Εu‚«
            }
            else
            {
                proxy.AccumulateValue(BlendShapePreset.Blink, 0);
            }
        }
        deltaTime+= Time.deltaTime;
        //‚±‚±‚ά‚Ε

        proxy.Apply();
    }
    private void AutoBlink()
    {
        
    }

    //public void ChangeFace(BlendShapePreset preset = BlendShapePreset.Neutral, bool blink = false)
    //{
    //    blinkEnabled = blink;

    //    if (!blink)
    //    {
    //        StopCoroutine("AutoBlink"); //•¶—ρ‚Εw’θ‚µ‚Θ‚Ά‚Ζ~‚ά‚η‚Θ‚Ά‚Μ‚Ε’Σ
    //        blinking = false;
    //        proxy.AccumulateValue(BlendShapePreset.Blink, 0);
    //    }

    //    proxy.AccumulateValue(currentFace, 0);  //΅‚Μ•\ξ‚π–³ψ‰»‚·‚ι
    //    proxy.AccumulateValue(preset, 1);    //V‚µ‚Ά•\ξ‚πƒZƒbƒg‚·‚ι

    //    currentFace = preset;
    //}

}