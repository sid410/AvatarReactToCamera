using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class LEDSender : MonoBehaviour
{
    //public Slider Slider;

    private string a = "0";
    private string b = "0";

    int LOCA_LPORT = 50000;
    static UdpClient udp;
    Thread thread;

    UdpClient udpClient;
    int tickRate = 10;
    Thread receiveThread;
    IPEndPoint receiveEP = new IPEndPoint(IPAddress.Any, 10002);
    IPEndPoint SendeIPEND;

    public bool Connect;
    //public Button Button;
    public string SendMesseage;
    //public GameObject StateManager;

    /* K = Tsunderader M = Omakase
    public Color State0;//Stanby
    public Color State1K;//Start
    public Color State2K;//TD1
    public Color State3K;//TD2
    public Color State4K;//Review
    public Color State5K;//Finish
    public bool Separate;
    public Color State1M;
    public Color State2M;
    public Color State3M;
    public Color State4M;
    public Color State5M;
    public Color State6;
    public Color State7;

    private Color State1;
    private Color State2;
    private Color State3;
    private Color State4;
    private Color State5;*/

    public Color Waiting;
    

    public Color NowColor;
    private Color PrevColor;

    public bool MaintananceMode;
    public Color ForMaintanance;

    //public GameObject Kaguya;
    //public GameObject manaka;

    [SerializeField, Range(0, 100)]
    public int Volume;

    [SerializeField, Range(0.01f, 10.0f)]
    public float CTime;

    private int State;

    private int PrevState;

    private GameObject Cube;
    private float ChangeColorTime;
    public float TimeOfChange;
    public float LoopTime;
    private string StateString;
    private bool Recieve;

    private bool UntilChangeColor;

    public bool StartChangeColor;
    const float FIXED_UPDATE_DELTATIME = 0.02f;

    public float BackColorTime;



    private void Start()
    {
        
    }
    void Awake()
    {
        udpClient = new UdpClient(receiveEP);

        receiveThread = new Thread(new ThreadStart(ThreadReceive));
        receiveThread.Start();
        Debug.Log("受信セットアップ完了");
        NowColor = Color.black;
        //StartCoroutine(SendMessage());
        
        PrevState = 0;
        PrevColor = Color.black;
        State = 0;
    }

    void ThreadReceive()
    {
        while (true)
        {
            Recieve = true;
            IPEndPoint senderEP = null;
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref senderEP);
                Parse(senderEP, receivedBytes);
            }
            catch (SocketException)
            {
                break;
            }
            
            
        }
        Recieve = false;
        
    }
    public void daipan()
    {
        StartCoroutine(SoundPlay());
    }

    IEnumerator SoundPlay()
    {
        
        b = "1";
        yield return new WaitForSeconds(0.7f);
    }

    void Parse(IPEndPoint senderEP, byte[] message)
    {
        string m = Encoding.UTF8.GetString(message);
        Debug.Log(m);
        if(m == "Connect")
        {
            byte[] SendM = Encoding.UTF8.GetBytes("ClariS");
            SendeIPEND = new IPEndPoint(senderEP.Address, LOCA_LPORT);

            udpClient.Send(SendM, SendM.Length, SendeIPEND);
            Connect = true;
            
            Debug.Log("Responce Message");
        }
    }

    private void Update()
    {
        if (!Recieve)
        {
            receiveThread.Abort();
            receiveThread = new Thread(new ThreadStart(ThreadReceive)); 
            receiveThread.Start(); 
        }


        StateString =this.GetComponent<main>().CurrentState;
        if (StateString == "00_Stanby") { State = 14; }
        else if (StateString == "01_Notice") { State = 0; }
        else if (StateString == "02_MoveFrameOut1") { State = 1; }
        else if (StateString == "03_MoveFrameIn1") { State = 2; }
        else if (StateString == "04_Greeting1") { State = 3; }
        else if (StateString == "05_Tsunderador1") { State = 4; }
        else if (StateString == "05_Omakase1") { State = 5; }
        else if (StateString == "06_WaitingForResponseTD") { State = 6; }
        else if (StateString == "06_WaitingForResponseOM") { State = 7; }
        else if (StateString == "07_Tsunderador2") { State = 8; }
        else if (StateString == "07_Omakase2") { State = 9; }
        else if (StateString == "idleAfterTD") { State = 10; }
        else if (StateString == "08_Greeting2") { State = 11; }
        else if (StateString == "09_Greeting2CCOCheki") { State = 12; }

        else State=13;

        
        Debug.Log("LEDSender: " + State);


        switch (State)
        {
            case 0:
                if (State == PrevState)
                {
                    ChangeColorTime += Time.deltaTime / TimeOfChange;
                    if (ChangeColorTime >= 1)
                    {
                        ChangeColorTime = 1;
                    }
                    //Debug.Log("Time delta: " + NowColor);
                    NowColor = Color.Lerp(PrevColor, Waiting, ChangeColorTime);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColorTime = 0;

                }
                break;

            case 10:
                if (State == PrevState)
                {
                    
                }
                else
                {
                    StartCoroutine(Yuragi());
                }



                break;
        }

        

        /*switch (State)
        {
            case 0:
                if(State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if(ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    Debug.Log("Time delta: " + NowColor);
                    NowColor = Color.Lerp(PrevColor, State0, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;

                }


                break;

            case 1:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State1, ChangeColor);

                }
                else
                {
                    StartCoroutine(SoundPlay());
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;

            case 2:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State2, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;

            case 3:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State3, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;

            case 4:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State4, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;

            case 5:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State5, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;

            case 6:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State6, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;

            case 7:
                if (State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if (ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
                    NowColor = Color.Lerp(PrevColor, State7, ChangeColor);

                }
                else
                {
                    PrevColor = NowColor;
                    PrevState = State;
                    ChangeColor = 0;
                }
                break;
        }*/
        //SendMesseage = NowColor.r.ToString() + "," + NowColor.g.ToString() + "," + NowColor.b.ToString() + "," + b;

        //Cube.GetComponent<Renderer>().material.color = NowColor;
        
        if (StartChangeColor)
        {
            //StartCoroutine(Yuragi());
            b = "1";
            StartChangeColor = false;
        }


        if (Connect && !MaintananceMode) // Normal mode
        {
            //Color color = Color.HSVToRGB(float.Parse(a), 1, 1);
            SendMesseage = NowColor.r.ToString() + "," + NowColor.g.ToString() + "," + NowColor.b.ToString() + "," + b + "," + Volume.ToString() + "," + "1";
            byte[] sendbyte = Encoding.UTF8.GetBytes(SendMesseage);
            udpClient.Send(sendbyte, sendbyte.Length, SendeIPEND);
            b = "0";
        }
        else if(Connect && MaintananceMode)//Maintanance Mode
        {
            SendMesseage = ForMaintanance.r.ToString() + "," + ForMaintanance.g.ToString() + "," + ForMaintanance.b.ToString() + "," + b + "," + Volume.ToString() + "," + "1";
            byte[] sendbyte = Encoding.UTF8.GetBytes(SendMesseage);
            udpClient.Send(sendbyte, sendbyte.Length, SendeIPEND);
            b = "0";
        }
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
    }

    /*public void GetIF()
    {
        a = Slider.value.ToString();
    }

    public void GetBut()
    {
        b = "1";
    }*/

    public void ColorChange(Color TargetColor, float ChangeTime)
    {
        if (!UntilChangeColor)
        {
            StartCoroutine(ColorChangeEmu(TargetColor, ChangeTime));
        }
    }

    public void ColorChangeTest()
    {
        if (!UntilChangeColor)
        {
            StartCoroutine(ColorChangeEmu(Color.red, 2f));
        }
    }

    IEnumerator ColorChangeEmu(Color TargetColor,float ChangeTime)
    {
        UntilChangeColor = true;
        PrevColor = NowColor;
        float TimeDis = 0;
        Debug.Log("CallChange"+ TargetColor) ;
        while (ChangeColorTime / ChangeTime <= 1.0f)
        {
            TimeDis = 0.02f;
            ChangeColorTime += TimeDis;
            //Debug.Log("Time delta: " + NowColor +  " " + ChangeColorTime.ToString());
            NowColor = Color.Lerp(PrevColor, TargetColor, Mathf.Clamp01(ChangeColorTime/ChangeTime));
            yield return new WaitForFixedUpdate();
        }
        ChangeColorTime = 0;
        UntilChangeColor = false;
        
    }

    IEnumerator Yuragi()
    {
        UntilChangeColor = true;
        float TimePool = 0;
        float ChangeRate = 0;
        Color BaseColor = NowColor;
        while(State != 14)
        {

            ChangeRate = (40 * Mathf.Cos(2 * Mathf.PI * TimePool / LoopTime) + 60)/100;
            //Debug.Log(ChangeRate.ToString());
            NowColor = new Color(BaseColor.r * ChangeRate, BaseColor.g * ChangeRate, BaseColor.b * ChangeRate);
            TimePool += 0.02f;
            yield return new WaitForFixedUpdate();
        }
        float EndTime = TimePool + BackColorTime;
        while(TimePool <= EndTime)
        {
            ChangeRate = (40 * Mathf.Cos(2 * Mathf.PI * TimePool / LoopTime) + 60) / 100;
            NowColor = new Color(BaseColor.r * ChangeRate, BaseColor.g * ChangeRate, BaseColor.b * ChangeRate);
            TimePool += 0.02f;
            yield return new WaitForFixedUpdate();
        }
        UntilChangeColor = false;
        NowColor = Color.black;
    }
}
