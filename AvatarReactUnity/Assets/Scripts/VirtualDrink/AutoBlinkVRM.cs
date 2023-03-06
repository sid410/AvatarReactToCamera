using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class AutoBlinkVRM : MonoBehaviour
{

    [SerializeField]
    float blinkTime = 0.1f;
    [SerializeField]
    float blinkInterval = 3.0f;

    bool blinkEnabled = true;
    bool blinking = false;
    BlendShapePreset currentFace;
    VRMBlendShapeProxy proxy;

    void Start()
    {
        proxy = GetComponent<VRMBlendShapeProxy>();

        //�f�t�H���g�̕\����Z�b�g
        currentFace = BlendShapePreset.Neutral;
        proxy.AccumulateValue(currentFace, 1);
    }

    void Update()
    {
        StartCoroutine("AutoBlink");  //������Ŏw�肷��K�v����

        proxy.Apply();
    }

    public void ChangeFace(BlendShapePreset preset = BlendShapePreset.Neutral, bool blink = false)
    {
        blinkEnabled = blink;

        if (!blink)
        {
            StopCoroutine("AutoBlink"); //������Ŏw�肵�Ȃ��Ǝ~�܂�Ȃ��̂Œ���
            blinking = false;
            proxy.AccumulateValue(BlendShapePreset.Blink, 0);
        }

        proxy.AccumulateValue(currentFace, 0);  //���̕\��𖳌�������
        proxy.AccumulateValue(preset, 1);    //�V�����\����Z�b�g����

        currentFace = preset;
    }

    IEnumerator AutoBlink()
    {
        if (!blinkEnabled | blinking)
        {
            yield break;
        }

        blinking = true;

        proxy.AccumulateValue(BlendShapePreset.Blink, 0);
        blinkInterval = Random.Range(3, 7);//3-6�b�̊ԂŃ����_���ŏu��

        yield return new WaitForSeconds(blinkInterval);

        proxy.AccumulateValue(BlendShapePreset.Blink, 1);

        yield return new WaitForSeconds(blinkTime);

        blinking = false;
    }

}