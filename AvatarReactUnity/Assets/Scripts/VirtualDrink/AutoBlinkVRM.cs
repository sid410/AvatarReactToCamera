using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class AutoBlinkVRM : MonoBehaviour
{

    [SerializeField]
    float blinkTime = 0.1f;//�ڂ��҂��Ă��鎞��
    [SerializeField]
    float blinkInterval = 1.0f;//�u���Əu���̊Ԃ̎���

    bool blinkEnabled = true;
    bool blinking = false;//true: �u����,false: �u�����Ă��Ȃ��Ƃ�
    BlendShapePreset currentFace;
    VRMBlendShapeProxy proxy;
    float deltaTime;

    void Start()
    {
        proxy = GetComponent<VRMBlendShapeProxy>();

        //�f�t�H���g�̕\����Z�b�g
        currentFace = BlendShapePreset.Neutral;
        proxy.AccumulateValue(currentFace, 1);
        Debug.Log("Test: Start");

        deltaTime = 0.0f;
    }


    void FixedUpdate()
    {
        //AutoBlink
        //��������
        if (blinking) {//�u�����Ă���
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
        else//�u�����Ă��Ȃ�
        {
            if (deltaTime > blinkInterval)
            {
                deltaTime = 0.0f;
                blinking = true;
                blinkInterval = Random.Range(2, 6);//2-5�b�̊ԂŃ����_���ŏu��
            }
            else
            {
                proxy.AccumulateValue(BlendShapePreset.Blink, 0);
            }
        }
        deltaTime+= Time.deltaTime;
        //�����܂�

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
    //        StopCoroutine("AutoBlink"); //������Ŏw�肵�Ȃ��Ǝ~�܂�Ȃ��̂Œ���
    //        blinking = false;
    //        proxy.AccumulateValue(BlendShapePreset.Blink, 0);
    //    }

    //    proxy.AccumulateValue(currentFace, 0);  //���̕\��𖳌�������
    //    proxy.AccumulateValue(preset, 1);    //�V�����\����Z�b�g����

    //    currentFace = preset;
    //}

}