using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Text;

public class Socketpp : MonoBehaviour
{
    // Start is called before the first frame update
    private Socket sock;
    public string receiveMsg;
    public string player_nickname;
    public string other_nickname;
    public string snapshot_timestamp;
    public string snapshot_intro;
    //public int snapshot_like;
    public int player_uid;
    public int other_player_uid;
    public string timeStamp = "Not";


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(IPAddress.Parse("34.64.92.185"), 5500);
        sock.Connect(ep);
        string cmd = string.Empty;
        //Debug.Log("Connected... Enter Q to exit");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string socket(string cmd)
    {
        byte[] receiverBuff = new byte[8192];
        Debug.Log("Connected... Enter Q to exit");
        // Q 를 누를 때까지 계속 Echo 실행
        byte[] buff = Encoding.UTF8.GetBytes(cmd);
        // (3) 서버에 데이타 전송
        sock.Send(buff, SocketFlags.None);
        // (4) 서버에서 데이타 수신
        int n = sock.Receive(receiverBuff);
        string data = Encoding.UTF8.GetString(receiverBuff, 0, n);

        return data;
    }


    public string cmd1;
    public string cmd2;
    public void ImageSend()
    {
        byte[] receiverBuff = new byte[5000000];
        Debug.Log("Connected... Enter Q to exit");
        // Q 를 누를 때까지 계속 Echo 실행
        byte[] buff = Encoding.UTF8.GetBytes(cmd1);
        // (3) 서버에 데이타 전송
        sock.Send(buff, SocketFlags.None);
        // (4) 서버에서 데이타 수신
        int n = sock.Receive(receiverBuff);
        Debug.Log("됨");
        buff = Encoding.UTF8.GetBytes(cmd2);
        Debug.Log("됨2");

        // (3) 서버에 데이타 전송
        sock.Send(buff, SocketFlags.None);
        Debug.Log("됨3");

        // (4) 서버에서 데이타 수신
        n = sock.Receive(receiverBuff);
        Debug.Log("됨4");

        int height = 255;
        int width = 255;
        Texture2D target = new Texture2D(height, width);
        target.LoadRawTextureData(buff);
        target.Apply();
        byte[] bytes = target.EncodeToPNG();
        Debug.Log(Application.dataPath);
        string path = Application.dataPath + "/absd.png";
        System.IO.File.WriteAllBytes(path, bytes);

        //string data = Encoding.UTF8.GetString(receiverBuff, 0, n);
    }
    /*
    public byte[] imageToByteArray(System.Drawing.Image imageIn)
    {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        return ms.ToArray();
    }
    public Image ByteArrayToImage(byte[] bytes)
    {
        MemoryStream ms = new MemoryStream(bytes);
        Image recImg = Image.FromStream(ms);
        return recImg;
    }*/
}
