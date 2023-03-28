using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjCam : MonoBehaviour
{
    public int MonitorSize;
    public float PlayerDistance;
    public float FOV;
    public GameObject cam;
    public GameObject screen;
    public GameObject TextureRender;
    public float monitor;
    
    //Unity�\��
    //�G���v�e�B
    //�[Screen�I�u�W�F�N�g�iPlane�j
    //�[Camera�I�u�W�F�N�g�iCamera�j


    // Start is called before the first frame update
    void Start()
    {
        cam = transform.Find("Camera").gameObject;
        screen = transform.Find("CalibScreen").gameObject;
        TextureRender = transform.Find("TexRenderer").gameObject;
        monitor = MonitorSize * 2.54f / 100 * Mathf.Cos(Mathf.Atan2(9f,16f));
        screen.GetComponent<Transform>().localScale = new Vector3(monitor/10f, 1f, monitor * 9f / 16f / 10f);
        TextureRender.GetComponent<Transform>().localScale = new Vector3(monitor / 10f, 1f, monitor * 9f / 16f / 10f);
        PlayerDistance = (MonitorSize * 2.54f/100f * Mathf.Cos(Mathf.Atan2(9f, 16f)) / 2f)/Mathf.Tan(FOV*Mathf.PI/360f);
        cam.GetComponent<Camera>().nearClipPlane = PlayerDistance;
        cam.GetComponent<Camera>().fieldOfView = Camera.HorizontalToVerticalFieldOfView(FOV, 16f / 9f);
        cam.GetComponent<Transform>().localPosition = new Vector3(0.0f, 0.0f,-1f*PlayerDistance);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
