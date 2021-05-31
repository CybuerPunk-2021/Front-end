using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Storage;
using Firebase.Database;
using System.Threading.Tasks;
using System.Threading;

public class Timeline : MonoBehaviour
{
    public GameObject snapshot;
    public int snapshot_count = 0;

    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        OnClickLoginAnonymous();
        timeline_client_to_server();
    }
    // Update is called once per frame
    void Update()
    {
    }

    public Firebase.Auth.FirebaseUser newUser;

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

    public void localDown()
    {
        string local_url = "c:\\Users\\secur\\Downloads\\test.png";
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://decisive-sylph-308301.appspot.com");
        StorageReference island_ref = storageRef.Child("test.png");

        Task task = island_ref.GetFileAsync(local_url, new StorageProgress<DownloadState>((DownloadState state) =>
        {
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred", state.BytesTransferred, state.TotalByteCount));
        }), CancellationToken.None);

        task.ContinueWith(resultTask =>
        {
            if (!resultTask.IsFaulted && !resultTask.IsCanceled)
                Debug.Log("OK");
        });
    }

    public void OnClickFileUpload()
    {
        string local_url = "c:\\Users\\secur\\Downloads\\dARak\\cookie\\test.png";

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://decisive-sylph-308301.appspot.com");
        StorageReference default_ref = storageRef.Child("test2.png");

        default_ref.PutFileAsync(local_url).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
                Debug.Log("ERR");
            else
            {
                StorageMetadata metadata = task.Result;
                Debug.Log("OK");
            }
        });
    }

    public void UpdateBtn()
    {
        timeline_client_to_server();
    }

    public void timeline_client_to_server()
    {
        Timeline_client_to_server timeline = new Timeline_client_to_server();
        timeline.uid = socketpp.player_uid;
        timeline.count = snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(timeline));
        Timeline_server_to_client snapshot = JsonUtility.FromJson<Timeline_server_to_client>(socketpp.receiveMsg);
        snapshot_count++;
        for (int i = 0; i < 2; i++)
        {
            MakeClone(snapshot.info[i].uid, snapshot.info[i].nickname, snapshot.info[i].timestamp, snapshot.info[i].snapshot_intro, snapshot.info[i].like);
        }
        //localDown();
        OnClickFileUpload();
    }

    public void MakeClone(int snapshot_user_uid, string snapshot_nickname, string snapshot_timestamp, string snapshot_text, string snapshot_like)
    {
        GameObject clone_snapshot = Instantiate(snapshot) as GameObject;
        clone_snapshot.transform.SetParent(this.transform);
        clone_snapshot.transform.localPosition = Vector3.zero;
        clone_snapshot.transform.localScale = Vector3.one;

        clone_snapshot.GetComponent<PrefabUid>().uid = snapshot_user_uid;
        clone_snapshot.GetComponent<PrefabUid>().nickname = snapshot_nickname;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_uid = snapshot_timestamp;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_intro = snapshot_text;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_like = snapshot_like;

        GameObject clone_snapshot_profile = clone_snapshot.transform.Find("ProfileImage").gameObject;
        clone_snapshot_profile.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        //GameObject profile_image = clone_snapshot.transform.Find("ProfileImage").gameObject;
        //profile_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_snapshot_snapshot = clone_snapshot.transform.Find("SnapshotImage").gameObject;
        clone_snapshot_snapshot.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().OnSnapshotScene());
        //GameObject snapshot_image = clone_snapshot.transform.Find("SnapshotImage").gameObject;
        //snapshot_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_snapshot_text = clone_snapshot.transform.Find("ProfileText").gameObject;
        clone_snapshot_text.GetComponent<Text>().text = snapshot_nickname;
    }
}
