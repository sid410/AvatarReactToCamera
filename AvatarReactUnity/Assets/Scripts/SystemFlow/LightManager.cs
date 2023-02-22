using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public GameObject Kaguya;
    public Color startColor;
    public Color endColor;

    
    private string KaguyaState;
    private GameObject[] lightObjects;
    private float speed;
    static float t = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        string[] lightObjectName = new string[14] {
            "Point.001",
            "Point.002",
            "Point.003",
            "Point.004",
            "Point.005",
            "Point.006",
            "Point.007",
            "Point.008",
            "Point.009",
            "Point.010",
            "Point.011",
            "Point.012",
            "Point.013",
            "Point.014"};

        lightObjects = new GameObject[14];
        for (int i = 0; i < lightObjectName.Length; i++)
        {
            lightObjects[i] = GameObject.Find(lightObjectName[i]);
            lightObjects[i].GetComponent<Light>().color = startColor;
        }

        speed = 0.1f;

    }

    // Update is called once per frame
    void Update()
    {
        KaguyaState = Kaguya.GetComponent<TsunderadorMenu>().state.getState();

        if (KaguyaState == "TsunderadorSecond")
        {
            float lightT = Mathf.Lerp(0.0f, 1.0f, t);
            for (int i = 0; i < lightObjects.Length; i++)
                lightObjects[i].GetComponent<Light>().color = Color.Lerp(startColor, endColor, lightT);
            Debug.Log("time: " + lightT);
            t += speed * Time.deltaTime;
        }

    }
}
