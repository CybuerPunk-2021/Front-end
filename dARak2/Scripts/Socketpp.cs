using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Text;
using Firebase.Storage;
using Firebase.Database;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

public class Socketpp : MonoBehaviour
{
    // Start is called before the first frame update
    private Socket sock;
    public string receiveMsg;
    public string player_nickname;
    public string other_nickname;
    public string player_email;
    public string player_password;
    public string player_recent_timestamp;
    public string player_profile_timestamp;
    public int player_profile_size;
    public string snapshot_timestamp = "Not";
    public string snapshot_intro;
    public int snapshot_like;
    public int player_uid;
    public int other_player_uid;
    public int tutorial_flag;
    //public string timeStamp = "Not";
    public Firebase.Auth.FirebaseUser newUser;
    public List<ImgQueue> _imgqueue;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        _imgqueue = new List<ImgQueue>();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(IPAddress.Parse("34.64.92.185"), 5500);
        sock.Connect(ep);
        string cmd = string.Empty;
        OnClickLoginAnonymous();
        //Debug.Log("Connected... Enter Q to exit");
    }

    void Update()
    {
        foreach (Socketpp.ImgQueue iq in _imgqueue)
        {
            if (File.Exists(iq.path))
            {
                FileInfo info = new FileInfo(iq.path);

                Debug.Log("INFO: " + info.Length.ToString() + ", iq: " + iq.size.ToString());
                if ((int)info.Length == iq.size)
                {
                    iq.img.sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(iq.path);
                    _imgqueue.Remove(iq);
                }
            }
            break;
        }
    }
    public class ImgQueue
    {
        public Image img;
        public string path;
        public int size;
    }

    public string socket(string cmd)
    {
        byte[] receiverBuff = new byte[163840];
        byte[] buff = Encoding.UTF8.GetBytes(cmd);
        sock.Send(buff, SocketFlags.None);
        int n = sock.Receive(receiverBuff);
        string data = Encoding.UTF8.GetString(receiverBuff, 0, n);

        return data;
    }

    public void OnClickLoginAnonymous()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    //파이어베이스에서 로컬로
    public void localDown(string local_url)
    {
        if (newUser == null)
            Debug.Log("NOT LOGIN");

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://decisive-sylph-308301.appspot.com/");
        StorageReference island_ref = storageRef.Child(local_url);
        island_ref.GetFileAsync(Application.persistentDataPath + "/" + local_url).ContinueWith(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log(local_url + ": OK");
            }
        });
    }


    //로컬에서 파이어베이스로
    public void OnClickFileUpload(string local_url, byte[] custom_bytes)
    {
        if (newUser == null)
            Debug.Log("NOT LOGIN");

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://decisive-sylph-308301.appspot.com/");
        StorageReference default_ref = storageRef.Child(local_url);

        default_ref.PutBytesAsync(custom_bytes).ContinueWith((Task<StorageMetadata> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                Firebase.Storage.StorageMetadata metadata = task.Result;
                Debug.Log("Finished uploading...");
            }
        });
    }

    IEnumerator Wait(ImgQueue iq)
    {
        yield return new WaitForSeconds(10f);
        iq.img.sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(iq.path);
        _imgqueue.Remove(iq);

    }

    IEnumerator OneFrame(ImgQueue iq)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
    }
}
