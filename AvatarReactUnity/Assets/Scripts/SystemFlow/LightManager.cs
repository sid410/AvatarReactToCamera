using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public GameObject Kaguya;

    private Color lightColor;
    private float colorValue;
    private string KaguyaState;
    private GameObject[] lightObjects;

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
            lightObjects[i].GetComponent<Light>().color = new Color(1.0f, 1.0f, 0.40f, 1.0f);
        }
        colorValue = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        KaguyaState = Kaguya.GetComponent<TsunderadorMenu>().state.getState();

        if (KaguyaState == "TsunderadorSecond" && colorValue <= 0.60f)
        {
            for (int i=0; i < lightObjects.Length; i++)
                lightObjects[i].GetComponent<Light>().color = new Color(1.0f, 1.0f - colorValue, 0.40f + colorValue);
            colorValue += 0.001f;
        }
    }
}
