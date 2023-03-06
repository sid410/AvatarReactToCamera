using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjMonitor : MonoBehaviour
{
    public int CenterMonitorSize;
    public int SubMonitorSize;
    public float Distance_CMonitor;
    public float Distance_SMonitor;

    private float CMonitorLength;
    private float SMonitorLength;

    public float HalfFoV_CMonitor;
    public float FoV_SMonitor;

    private float Distance_Center;
    private float Distance_Edge;

    public bool ADJmode;

    public GameObject CMonitor;
    public GameObject SMonitorL;
    public GameObject SMonitorR;

    public float CMonitor_Height;
    public float SMonitorL_Height;
    public float SMonitorR_Height;

    public GameObject Ccam;
    public GameObject ScamL;
    public GameObject ScamR;


    // Start is called before the first frame update
    void Start()
    {
        if (ADJmode)//When you set monitor and camera position
        {
            CMonitorLength = CenterMonitorSize * 2.54f / 100 * Mathf.Cos(Mathf.Atan2(9f, 16f));
            SMonitorLength = SubMonitorSize * 2.54f / 100 * Mathf.Cos(Mathf.Atan2(9f, 16f));
            Distance_Center = Distance_CMonitor + Distance_SMonitor;

            HalfFoV_CMonitor = Mathf.Atan((CMonitorLength / 2) / Distance_CMonitor);
            float a = Mathf.Tan(HalfFoV_CMonitor) * (Distance_Center);
            Distance_Edge = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(Distance_Center, 2));

            FoV_SMonitor = Mathf.Acos((2 * Mathf.Pow(Distance_Edge, 2) - Mathf.Pow(SMonitorLength, 2)) / (2 * Mathf.Pow(Distance_Edge, 2)));

            CMonitor.transform.localPosition = new Vector3(0, CMonitor_Height, Distance_CMonitor);
            SMonitorL.transform.localPosition = new Vector3(Distance_Center * Mathf.Sin(-1 * (HalfFoV_CMonitor + FoV_SMonitor / 2)), SMonitorL_Height, Distance_Center * Mathf.Cos(HalfFoV_CMonitor + FoV_SMonitor / 2));
            SMonitorR.transform.localPosition = new Vector3(Distance_Center * Mathf.Sin((HalfFoV_CMonitor + FoV_SMonitor / 2)), SMonitorR_Height, Distance_Center * Mathf.Cos(HalfFoV_CMonitor + FoV_SMonitor / 2));

            SMonitorL.transform.rotation = Quaternion.Euler(new Vector3(0, (HalfFoV_CMonitor + FoV_SMonitor / 2) * -360/(2 * Mathf.PI), 0));
            SMonitorR.transform.rotation = Quaternion.Euler(new Vector3(0, (HalfFoV_CMonitor + FoV_SMonitor / 2) * 360 / (2 * Mathf.PI), 0));

            CMonitor.GetComponent<Transform>().localScale = new Vector3(CMonitorLength / 10f, CMonitorLength * 9f / 16f / 10f, 1f);
            SMonitorL.GetComponent<Transform>().localScale = new Vector3(SMonitorLength / 10f, SMonitorLength * 9f / 16f / 10f, 1f);
            SMonitorR.GetComponent<Transform>().localScale = new Vector3(SMonitorLength / 10f, SMonitorLength * 9f / 16f / 10f, 1f);

            Ccam.transform.rotation = Quaternion.FromToRotation(Vector3.forward, CMonitor.transform.position);
            ScamL.transform.rotation = Quaternion.FromToRotation(Vector3.forward, SMonitorL.transform.position);
            ScamR.transform.rotation = Quaternion.FromToRotation(Vector3.forward, SMonitorR.transform.position);

            Ccam.GetComponent<Camera>().fieldOfView = Camera.HorizontalToVerticalFieldOfView((HalfFoV_CMonitor*360 * 2 )/ (2*Mathf.PI), 16f / 9f);
            ScamL.GetComponent<Camera>().fieldOfView = Camera.HorizontalToVerticalFieldOfView(FoV_SMonitor * 360 / (2 * Mathf.PI), 16f / 9f);
            ScamR.GetComponent<Camera>().fieldOfView = Camera.HorizontalToVerticalFieldOfView(FoV_SMonitor * 360 / (2 * Mathf.PI), 16f / 9f);



        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
