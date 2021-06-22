using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class FriendPageScript : MonoBehaviour
{
    public GameObject ProfileImage, ProfileName, ProfilewallpaperImage, ProfileFollower, ProfileText, SnapshotsImage;
    public GameObject post;
    public GameObject popup;
    public InputField PostInputField;
    public int visitbook_count = 0; //방명록 리스트 카운트
    Socketpp socketpp;
    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓불러오기
    }
    //켜질시
    void OnEnable()
    {
        ErasePost(); //방명록 삭제하기
        profile_client_to_server(); // 프로필 불러오기
        loadvisitbook_client_to_server(); //방명록 불러오기
    }

    //프로필 재로딩
    public void reloadFriendPage()
    {
        ErasePost(); //방명록 삭제하기
        profile_client_to_server(); // 프로필 불러오기
        loadvisitbook_client_to_server(); //방명록 불러오기
    }
    //프로필 불러오기
    public void profile_client_to_server()
    {
        Profile_client_to_server profile = new Profile_client_to_server();
        profile.uid = socketpp.other_player_uid;
        if (!Directory.Exists(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/");//플레이어uid폴더 없을 시 생성
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profile)); //다른사용자 uid 클라이언트에서 서버로 전달
        Profile_server_to_client myProfile = JsonUtility.FromJson<Profile_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        ProfileName.GetComponent<Text>().text = socketpp.other_nickname;
        ProfileFollower.GetComponent<Text>().text = myProfile.follower.ToString();
        ProfileText.GetComponent<Text>().text = myProfile.self_intro;
        //프로필 스냅샷 불러오기
        if (myProfile.snapshot_info.timestamp != "")
        {
            SnapshotsImage.SetActive(true);
            string path = socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + myProfile.snapshot_info.timestamp + ".png";
            if (!File.Exists(Application.persistentDataPath + "/" + path))
            {
                socketpp.localDown(path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = SnapshotsImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + path;
                iq.size = myProfile.snapshot_info.size;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                while (true)
                {
                    FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path);
                    if (info.Length == myProfile.snapshot_info.size)
                    {
                        SnapshotsImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
                        break;
                    }
                }
            }
        }
        else if (myProfile.snapshot_info.timestamp == "")
        {
            SnapshotsImage.SetActive(false);
        }
        //프로필 이미지 불러오기
        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = new int[1] { socketpp.other_player_uid };
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);
        if (checkprofiletime.timestamp[0] != "")
        {
            string profile_path = socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + checkprofiletime.timestamp[0] + ".png";
            if (!File.Exists(Application.persistentDataPath + "/" + profile_path))
            {
                socketpp.localDown(profile_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfileImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + profile_path;
                iq.size = checkprofiletime.size[0];
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                while (true)
                {
                    FileInfo info = new FileInfo(Application.persistentDataPath + "/" + profile_path);
                    if (info.Length == checkprofiletime.size[0])
                    {
                        ProfileImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + profile_path);
                        break;
                    }
                }
            }
        }
        else if (checkprofiletime.timestamp[0] == "")
        {
            string profile_path = "profile_default.png";
            if (!File.Exists(Application.persistentDataPath + "/" + profile_path))
            {
                socketpp.localDown(profile_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfileImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + profile_path;
                iq.size = checkprofiletime.size[0];
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                while (true)
                {
                    FileInfo info = new FileInfo(Application.persistentDataPath + "/" + profile_path);
                    if (info.Length == checkprofiletime.size[0])
                    {
                        ProfileImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + profile_path);
                        break;
                    }
                }
            }
        }
        //프로필 배경화면 불러오기
        CheckBackgroundImage_client_to_server checkbg = new CheckBackgroundImage_client_to_server();
        checkbg.uid = socketpp.other_player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkbg));
        CheckBackgroundImage_server_to_client checkbgtime = JsonUtility.FromJson<CheckBackgroundImage_server_to_client>(socketpp.receiveMsg);
        if (checkbgtime.timestamp != "")
        {
            string bg_path = socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + checkbgtime.timestamp + ".png";
            if (!File.Exists(Application.persistentDataPath + "/" + bg_path))
            {
                socketpp.localDown(bg_path);
                Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
                iq.img = ProfilewallpaperImage.GetComponent<Image>();
                iq.path = Application.persistentDataPath + "/" + bg_path;
                iq.size = checkbgtime.size;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                while (true)
                {
                    FileInfo info = new FileInfo(Application.persistentDataPath + "/" + bg_path);
                    if (info.Length == checkbgtime.size)
                    {
                        ProfilewallpaperImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + bg_path);
                        break;
                    }
                }
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
                iq.size = checkbgtime.size;
                socketpp._imgqueue.Add(iq);
            }
            else
            {
                while (true)
                {
                    FileInfo info = new FileInfo(Application.persistentDataPath + "/" + bg_path);
                    if (info.Length == checkbgtime.size)
                    {
                        ProfilewallpaperImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + bg_path);
                        break;
                    }
                }
            }
        }
        //스냅샷 이미지 정보 불러오기
        SnapshotsImage.GetComponent<PrefabUid>().uid = socketpp.other_player_uid;
        SnapshotsImage.GetComponent<PrefabUid>().nickname = socketpp.other_nickname;
        SnapshotsImage.GetComponent<SnapshotUid>().snapshot_uid = myProfile.snapshot_info.timestamp;
        SnapshotsImage.GetComponent<SnapshotUid>().snapshot_intro = myProfile.snapshot_info.snapshot_intro;
        SnapshotsImage.GetComponent<SnapshotUid>().snapshot_like = myProfile.snapshot_info.like_num;
    }
    //업데이트 버튼
    public void UpdatePostBtn()
    {
        loadvisitbook_client_to_server();//방명록 불러오기
    }
    //쓰기 버튼
    public void WritePostBtn()
    {
        writevisitbook_client_to_server(); //방명록 쓰기
        ErasePost();//방명록 삭제
        loadvisitbook_client_to_server();//방명록 불러오기
        popup.transform.GetChild(0).GetComponent<Text>().text = "작성 완료";
        popup.SetActive(true);
    }
    //방명록 삭제
    public void ErasePost()
    {
        GameObject[] posts = GameObject.FindGameObjectsWithTag("FriendPost");
        foreach (GameObject post in posts)
        {
            Destroy(post); //방명록 Prefab삭제
        }
        visitbook_count = 0;  //방명록 라스트 카운트 초기화
    }
    //방명록 불러오기
    public void loadvisitbook_client_to_server()
    {
        Loadvisitbook_client_to_server loadvisitbook = new Loadvisitbook_client_to_server();
        loadvisitbook.uid = socketpp.other_player_uid;
        loadvisitbook.count = visitbook_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(loadvisitbook)); //다른사용자uid 방명록 리스트 숫자 클라이언트에서 서버로 전달
        Loadvisitbook_server_to_client visitbook = JsonUtility.FromJson<Loadvisitbook_server_to_client>(socketpp.receiveMsg);//서버에서 받은 것을 클라이언트로 전달
        visitbook_count++;//방명록 리스트 증가
        //방명록 프로필 이미지 가져오기
        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        int[] uid_list = new int[visitbook.visit_book.Length];
        for (int i = 0; i < uid_list.Length; i++)
        {
            uid_list[i] = visitbook.visit_book[i].writer_uid;
        }
        checkprofile.uid = uid_list.Distinct().ToArray();
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);
        Dictionary<int, string> timedict = new Dictionary<int, string>();
        Dictionary<int, int> sizedict = new Dictionary<int, int>();

        for (int i = 0; i < checkprofile.uid.Length; i++)
        {
            timedict.Add(checkprofile.uid[i], checkprofiletime.timestamp[i]);
            sizedict.Add(checkprofile.uid[i], checkprofiletime.size[i]);
        }

        for (int i = 0; i < 5; i++)
        {
            MakePost(visitbook.visit_book[i].writer_uid, visitbook.visit_book[i].nickname, visitbook.visit_book[i].comment, visitbook.visit_book[i].timestamp, timedict[visitbook.visit_book[i].writer_uid], sizedict[visitbook.visit_book[i].writer_uid]);
        }
    }
    //방명록 Prefab생성
    public void MakePost(int visitbook_uid, string visitbook_nickname, string visitbook_comment, string visitbook_timestamp, string visitbook_writer_timestamp, int visitbook_writer_size)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + visitbook_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + visitbook_uid.ToString() + "/");//플레이어uid폴더 없을 시 생성
        GameObject clone_post = Instantiate(post) as GameObject;
        clone_post.transform.SetParent(this.transform);
        clone_post.transform.localPosition = Vector3.zero;
        clone_post.transform.localScale = Vector3.one;

        clone_post.GetComponent<PrefabUid>().uid = visitbook_uid;  //방명록 uid 
        clone_post.GetComponent<PrefabUid>().nickname = visitbook_nickname;//방명록 닉네임

        GameObject post_profile_image = clone_post.transform.Find("PostImage").gameObject;
        post_profile_image.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ActiveFriendPageToFriendPage()); //방명록 이미지 클릭시 페이지로 이동

        //방명록 프로필 사진 가져오기
        string path = visitbook_uid.ToString() + "/" + visitbook_uid.ToString() + "_" + visitbook_writer_timestamp + ".png";
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = post_profile_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            iq.size = visitbook_writer_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            while (true)
            {
                FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path);
                if (info.Length == visitbook_writer_size)
                {
                    post_profile_image.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
                    break;
                }
            }
        }
        GameObject Post_Name = clone_post.transform.Find("PostName").gameObject;
        Post_Name.GetComponent<Text>().text = visitbook_nickname;//방명록 쓴사람 이름 보여주기
        GameObject Post_Answer = clone_post.transform.Find("PostAnswer").gameObject;
        Post_Answer.GetComponent<Text>().text = visitbook_comment;//방명록 글 보여주기
        GameObject Post_Timestamp = clone_post.transform.Find("PostTime").gameObject;
        Post_Timestamp.GetComponent<Text>().text = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ParseDateTime(visitbook_timestamp);//방명록 시간 보여주기
    }
    //방명록 쓰기
    public void writevisitbook_client_to_server()
    {
        Writevisitbook_client_to_server writevisitbook = new Writevisitbook_client_to_server();
        writevisitbook.writer_uid = socketpp.player_uid;
        writevisitbook.uid = socketpp.other_player_uid;
        writevisitbook.comment = PostInputField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(writevisitbook));//작성자 사용자 uid 장소 사용자 uid 방명록 내용을 클라이언트에서 서버로 전달
        //Writevisitbook_server_to_client visitbook_time = JsonUtility.FromJson<Writevisitbook_server_to_client>(socketpp.receiveMsg);
        //MakePost(writevisitbook.writer_uid, socketpp.player_nickname, PostInputField.text, visitbook_time.timestamp, socketpp.player_profile_timestamp, socketpp.player_profile_size);
    }
}
