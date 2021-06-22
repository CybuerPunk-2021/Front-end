using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class FollowingScript : MonoBehaviour
{
    public GameObject friend;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓 불러오기
    }
    //켜질시
    void OnEnable()
    {
        UpdateFollowing(); //팔로잉 업데이트
    }
    //팔로잉 업데이트
    public void UpdateFollowing()
    {
        GameObject[] followings = GameObject.FindGameObjectsWithTag("Following");
        foreach (GameObject following in followings)
        {
            Destroy(following); //팔로잉 Prefab 전부 제거
        }
        followingscene_client_to_server(); //팔로잉 가져오기
    }

    //팔로잉 가져오기
    public void followingscene_client_to_server()
    {
        Followingscene_client_to_server following_scene = new Followingscene_client_to_server();
        following_scene.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(following_scene)); //현재 사용자 uid 클라이언트에서 서버로 전달
        Followingscene_server_to_client followings = JsonUtility.FromJson<Followingscene_server_to_client>(socketpp.receiveMsg);//서버에서 전달받은 것을 클라이언트로 전달

        //이미지 프로필 가져오기
        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        int[] uid_list = new int[followings.following.Length];
        for (int i = 0; i < uid_list.Length; i++)
        {
            uid_list[i] = followings.following[i].uid;
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

        for (int i = 0; i < 50; i++)
        {
            MakeFollowing(followings.following[i].uid, followings.following[i].nickname, timedict[followings.following[i].uid], sizedict[followings.following[i].uid]); //팔로잉 정보로 팔로잉 Prefab생성
        }
    }

    //팔로잉 Prefab생성
    public void MakeFollowing(int following_uid, string following_nickname, string following_profile_timestamp, int following_profile_size)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + following_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + following_uid.ToString() + "/"); //플레이어uid폴더 없을 시 생성
        GameObject clone_following_friend = Instantiate(friend) as GameObject;
        clone_following_friend.transform.SetParent(this.transform);
        clone_following_friend.transform.localPosition = Vector3.zero;
        clone_following_friend.transform.localScale = Vector3.one;
        clone_following_friend.GetComponent<PrefabUid>().uid = following_uid; //팔로잉 uid
        clone_following_friend.GetComponent<PrefabUid>().nickname = following_nickname; //팔로잉 닉네임
        GameObject clone_following_friend_button = clone_following_friend.transform.Find("FollowingPage").gameObject;
        clone_following_friend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ActiveFriendPage()); //팔로잉 페이지 들어가기 버튼 추가
        GameObject clone_following_friend_deletebutton = clone_following_friend.transform.Find("deleteButton").gameObject;
        clone_following_friend_deletebutton.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Friend").GetComponent<FriendScript>().followdelete_client_to_server()); //팔로잉 삭제 버튼 추가
        GameObject clone_following_text = clone_following_friend.transform.Find("FollowingName").gameObject;
        clone_following_text.GetComponent<Text>().text = following_nickname; //팔로잉 닉네임 텍스트로 보여주기

        //팔로잉 프로필 이미지 불러오기
        string path = following_uid.ToString() + "/" + following_uid.ToString() + "_" + following_profile_timestamp + ".png";
        GameObject clone_following_image = clone_following_friend.transform.Find("FollowingImage").gameObject;
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_following_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            iq.size = following_profile_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            while (true)
            {
                FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path);
                if (info.Length == following_profile_size)
                {
                    clone_following_image.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
                    break;
                }
            }
        }
    }
}