using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public class pythonProgram : MonoBehaviour
{
    //python������ꏊ
    private string pyExePath = @"C:/Users/imd-lab/AppData/Local/Programs/Python/Python39/python.exe";

    //���s�������X�N���v�g������ꏊ
    private string pyCodePath = @"C:/Users/imd-lab/Desktop/2022TSUNDERE/VirtualCafe/AvatarReactUnity/Assets/Scripts/Python/code.py";

    //�G�[�W�F���g�؂�ւ��̂��ߎg�p
    public main main;
    //�e�L�X�g�̉��H�O�̈�s������ϐ�
    public string[] textMessage;
 
    
    //�񓯊������ipython�R�[�h�̏������d���̂Łj
    public async void ChangeBehaviourBasedOnQuestionnaire()
    {
        string str = "";
        //���O�A���P�[�g���ʂ̓ǂݍ���
        TextAsset textasset = new TextAsset(); //�e�L�X�g�t�@�C���̃f�[�^���擾����C���X�^���X���쐬
        textasset = Resources.Load("Questionnaire/qResult", typeof(TextAsset)) as TextAsset; //Resources�t�H���_����Ώۃe�L�X�g���擾
        string TextLines = textasset.text; //�e�L�X�g�S�̂�string�^�œ����ϐ���p�ӂ��ē����
        textMessage = TextLines.Split('\n');
        if (textMessage[textMessage.Length - 1] == "")
        {
            textMessage = textMessage.SkipLast(1).ToArray();
            print("The last line is null.");
        }
        int textlength = textMessage.Length;
        string[] qResult = textMessage[textlength - 1].Split(',');
        if (main == null) main = this.GetComponent<main>();
        await Task.Run(() =>
        {
            // ����͕ʃX���b�h�Ŏ��{
            str = PythonCode(qResult[0], qResult[1], qResult[2], qResult[3]);
            return true;
        });
        //�ȍ~�̓��C���X���b�h�Ŏ��{
        switch (str)
        {
            case "deredere":
                print("Manaka is selected!");
                if (main.agentNum != 1)//not manaka
                {
                    main.SetAgent(); //change to manaka
                }
                break;
            case "tsundere":
                print("Kaguya is selected!");
                if (main.agentNum != 0)//not kaguya
                {
                    main.SetAgent(); //change to kaguya
                }
                break;
            default:
                print("Error: the output of 'code.py' is incorrect.");
                break;

        }
    }
    private string PythonCode(string qr0, string qr1, string qr2, string qr3)
    {

        //�O���v���Z�X�̐ݒ�
        ProcessStartInfo processStartInfo = new ProcessStartInfo()
        {
            FileName = pyExePath, //���s����t�@�C��(python)
            UseShellExecute = false,//�V�F���@�\���g�p���Ȃ�
            CreateNoWindow = true, //�R���\�[���E�E�B���h�E���J���Ȃ�
            RedirectStandardOutput = true, //�e�L�X�g�o�͂�StandardOutput�X�g���[���ɏ������ނ��ǂ���
            Arguments = pyCodePath + " " + qr0 + " " + qr1 + " " + qr2 + " " + qr3//���s����X�N���v�g �����Ƃ��ăA���P�[�g���ڂ̉񓚌��ʂ����
        };

        //�O���v���Z�X�̊J�n
        Process process = Process.Start(processStartInfo);

        //�X�g���[������o�͂𓾂�
        StreamReader streamReader = process.StandardOutput;
        string str = streamReader.ReadLine();

        //�O���v���Z�X�̏I��
        process.WaitForExit();
        process.Close();

        return str;
    }
}