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
    public Slider Slider;

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
    public Button Button;
    public string SendMesseage;
    public GameObject StateManager;

    public Color State0;
    public Color State1;
    public Color State2;
    public Color State3;
    public Color State4;
    public Color State5;

    private Color NowColor;
    private Color PrevColor;

    public int State;

    private int PrevState;

    public GameObject Cube;
    private float ChangeColor;
    public float TimeOfChange;
    private string StateString;


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
            IPEndPoint senderEP = null;
            byte[] receivedBytes = udpClient.Receive(ref senderEP);
            Parse(senderEP, receivedBytes);
        }
    }

    IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(2);
        b = "1";
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
        StateString = StateManager.GetComponent<TsunderadorMenu>().state.getState();
        if (StateString == "Start") { State = 0; }
        else if (StateString == "TsunderadorFirst") { State = 1; }
        else if (StateString == "TsunderadorSecond") { State = 2; }
        else if (StateString == "Finish") { State = 3; }
        else { State = 4; }

        switch (State)
        {
            case 0:
                if(State == PrevState)
                {
                    ChangeColor += Time.deltaTime / TimeOfChange;
                    if(ChangeColor >= 1)
                    {
                        ChangeColor = 1;
                    }
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

        }

        
        //Cube.GetComponent<Renderer>().material.color = NowColor;
        if (Connect)
        {
            //Color color = Color.HSVToRGB(float.Parse(a), 1, 1);
            SendMesseage = NowColor.r.ToString() + "," + NowColor.g.ToString() + "," + NowColor.b.ToString() + "," + b;
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
}
