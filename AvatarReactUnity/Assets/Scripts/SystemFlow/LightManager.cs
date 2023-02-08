using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public GameObject Kaguya;

    private Color lightColor;
    private float colorValue;
    private string KaguyaState;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Light>().color = new Color(255, 255, 100);
        colorValue = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        KaguyaState = Kaguya.GetComponent<TsunderadorMenu>().state.getState();

        if (KaguyaState == "TsunderadorSecond" && colorValue <= 155.0f)
        {
            this.gameObject.GetComponent<Light>().color = new Color(255, 255 - colorValue, 100 + colorValue);
            colorValue += 0.1f;
        }
    }
}
