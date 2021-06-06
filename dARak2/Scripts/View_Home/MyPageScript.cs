using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class MyPageScript : MonoBehaviour
{
    public GameObject ProfileImage, ProfileName, ProfilewallpaperImage, ProfileFollower, ProfileText, SnapshotsImage;
    public GameObject post;
    public InputField PostInputField;
    public int visitbook_count = 0;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        profile_client_to_server();
        loadvisitbook_client_to_server();
    }
    void OnEnable()
    {
        //profile_client_to_server();
        //ErasePost();
        //loadvisitbook_client_to_server();
    }

    public void ReloadProfile()
    {
        profile_client_to_server();
        ErasePost();
        loadvisitbook_client_to_server();
    }

    public void profile_client_to_server()
    {
        Profile_client_to_server profile = new Profile_client_to_server();
        profile.uid = socketpp.player_uid;
        if (!Directory.Exists(Application.persistentDataPath + "/" + socketpp.player_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + socketpp.player_uid.ToString() + "/");
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profile));
        Profile_server_to_client myProfile = JsonUtility.FromJson<Profile_server_to_client>(socketpp.receiveMsg);
        ProfileName.GetComponent<Text>().text = socketpp.player_nickname;
        ProfileFollower.GetComponent<Text>().text = myProfile.follower.ToString();
        ProfileText.GetComponent<Text>().text = myProfile.self_intro;
        if (myProfile.snapshot_info.timestamp != "")
        {
            SnapshotsImage.SetActive(true);
            string path = socketpp.player_uid.ToString() + "/" + socketpp.player_uid.ToString() + "_" + myProfile.snapshot_info.timestamp + ".png";
            if (!File.Exists(Application.persistentDataPath + "/" + path))
            {
                socketpp.localDown(path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = SnapshotsImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + path;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                SnapshotsImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
            }
        }
        else if (myProfile.snapshot_info.timestamp == "")
        {
            SnapshotsImage.SetActive(false);
        }

        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));

        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);
        if (checkprofiletime.timestamp != "")
        {
            string profile_path = socketpp.player_uid.ToString() + "/" + socketpp.player_uid.ToString() + "_" + checkprofiletime.timestamp + ".png";
            if (!File.Exists(Application.persistentDataPath + "/" + profile_path))
            {
                socketpp.localDown(profile_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfileImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + profile_path;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                ProfileImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + profile_path);
            }
        }
        else if (checkprofiletime.timestamp == "")
        {
            string profile_path = "profile_default.png";
            if (!File.Exists(Application.persistentDataPath + "/" + profile_path))
            {
                socketpp.localDown(profile_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfileImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + profile_path;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                ProfileImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + profile_path);
            }
        }

        CheckBackgroundImage_client_to_server checkbg = new CheckBackgroundImage_client_to_server();
        checkbg.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkbg));
        CheckBackgroundImage_server_to_client checkbgtime = JsonUtility.FromJson<CheckBackgroundImage_server_to_client>(socketpp.receiveMsg);
        if (checkbgtime.timestamp != "")
        {
            string bg_path = socketpp.player_uid.ToString() + "/" + socketpp.player_uid.ToString() + "_" + checkbgtime.timestamp + ".png";
            if (!File.Exists(Application.persistentDataPath + "/" + bg_path))
            {
                socketpp.localDown(bg_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfilewallpaperImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + bg_path;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                ProfilewallpaperImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + bg_path);
            }
        }
        else if (checkbgtime.timestamp == "")
        {
            string bg_path = "bg_default.jpg";
            if (!File.Exists(Application.persistentDataPath + "/" + bg_path))
            {
                socketpp.localDown(bg_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfilewallpaperImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + bg_path;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                ProfilewallpaperImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + bg_path);
            }
        }

        SnapshotsImage.GetComponent<PrefabUid>().uid = socketpp.player_uid;
        SnapshotsImage.GetComponent<PrefabUid>().nickname = socketpp.player_nickname;
        SnapshotsImage.GetComponent<SnapshotUid>().snapshot_uid = myProfile.snapshot_info.timestamp;
        SnapshotsImage.GetComponent<SnapshotUid>().snapshot_intro = myProfile.snapshot_info.snapshot_intro;
        SnapshotsImage.GetComponent<SnapshotUid>().snapshot_like = myProfile.snapshot_info.like_num;
    }

    public void UpdatePostBtn()
    {
        loadvisitbook_client_to_server();
    }
    public void WritePostBtn()
    {
        writevisitbook_client_to_server();
        ErasePost();
        loadvisitbook_client_to_server();
    }
    public void ErasePost()
    {
        GameObject[] posts = GameObject.FindGameObjectsWithTag("Post");
        foreach (GameObject post in posts)
        {
            Destroy(post);
        }
        visitbook_count = 0;
    }

    public void loadvisitbook_client_to_server()
    {
        Loadvisitbook_client_to_server loadvisitbook = new Loadvisitbook_client_to_server();
        loadvisitbook.uid = socketpp.player_uid;
        loadvisitbook.count = visitbook_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(loadvisitbook));
        Loadvisitbook_server_to_client visitbook = JsonUtility.FromJson<Loadvisitbook_server_to_client>(socketpp.receiveMsg);
        visitbook_count++;
        for (int i = 0; i < 5; i++)
        {
            MakePost(visitbook.visit_book[i].writer_uid, visitbook.visit_book[i].nickname, visitbook.visit_book[i].comment, visitbook.visit_book[i].timestamp);
        }
    }

    public void MakePost(int visitbook_uid, string visitbook_nickname, string visitbook_comment, string visitbook_timestamp)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + visitbook_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + visitbook_uid.ToString() + "/");

        GameObject clone_post = Instantiate(post) as GameObject;
        clone_post.transform.SetParent(this.transform);
        clone_post.transform.localPosition = Vector3.zero;
        clone_post.transform.localScale = Vector3.one;

        clone_post.GetComponent<PrefabUid>().uid = visitbook_uid;
        clone_post.GetComponent<PrefabUid>().nickname = visitbook_nickname;

        GameObject post_profile_image = clone_post.transform.Find("PostImage").gameObject;
        post_profile_image.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActivePostToFriendPage());

        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = visitbook_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);

        string path = visitbook_uid.ToString() + "/" + visitbook_uid.ToString() + "_" + checkprofiletime.timestamp + ".png";
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = post_profile_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            post_profile_image.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
        }

        GameObject Post_Name = clone_post.transform.Find("PostName").gameObject;
        Post_Name.GetComponent<Text>().text = visitbook_nickname;
        GameObject Post_Answer = clone_post.transform.Find("PostAnswer").gameObject;
        Post_Answer.GetComponent<Text>().text = visitbook_comment;
        GameObject Post_Timestamp = clone_post.transform.Find("PostTime").gameObject;
        Post_Timestamp.GetComponent<Text>().text = GameObject.Find("View_Main").GetComponent<MainSceneScript>().ParseDateTime(visitbook_timestamp);
    }

    public void writevisitbook_client_to_server()
    {
        Writevisitbook_client_to_server writevisitbook = new Writevisitbook_client_to_server();
        writevisitbook.writer_uid = socketpp.player_uid;
        writevisitbook.uid = socketpp.player_uid;
        writevisitbook.comment = PostInputField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(writevisitbook));
        Writevisitbook_server_to_client visitbook_time = JsonUtility.FromJson<Writevisitbook_server_to_client>(socketpp.receiveMsg);
        MakePost(writevisitbook.writer_uid, socketpp.player_nickname, PostInputField.text, visitbook_time.timestamp);
    }
}
