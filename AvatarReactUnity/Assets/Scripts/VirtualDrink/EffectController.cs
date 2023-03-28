using
    System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    private CameraShake shake;//�J������h�炷���߂̂���
    private LEDSender LED;
    // Start is called before the first frame update
    void Start()
    {
        if (shake == null) shake = Camera.main.GetComponent<CameraShake>();//�J������h�炷���߂̃N���X���p��
        LED = GameObject.Find("Main").GetComponent<LEDSender>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Daipan()
    {
        LED.daipan();
    }
    public void DaipanShake()
    {
        StartCoroutine(ShakeCamera());
    }
    //�J������h�炷�֐�
    IEnumerator ShakeCamera()
    {
        float time = 0.1f;
        shake.Shake(time, 0.03f);
        yield return new WaitForSecondsRealtime(time);
        Camera.main.transform.position = new Vector3(-2.45f, 1.3f, -10.85f);//�J�����������ʒu�ɖ߂�
        //Debug.Log("CameraPos: " + Camera.main.transform.position);
    }

    public void ColorChangeTest()
    {
        LED.ColorChange(Color.red, 2f);
    }

    public void ColorChangeTsunde()
    {
        LED.ColorChange(Color.blue , 2f);
    }

}
