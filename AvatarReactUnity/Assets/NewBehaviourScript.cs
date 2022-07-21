using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float rote = 30f;
    public float tote;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tote = rote * Time.deltaTime;
        transform.Rotate(new Vector3(0, tote, 0));
    }
}
