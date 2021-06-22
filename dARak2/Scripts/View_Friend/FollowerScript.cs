using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class FollowerScript : MonoBehaviour
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
        UpdateFollower(); //팔로워 업데이트
    }
    //팔로워 업데이트
    public void UpdateFollower()
    {
        GameObject[] followers = GameObject.FindGameObjectsWithTag("Follower");
        foreach (GameObject follower in followers)
        {
            Destroy(follower); //팔로워 Prefab 전부 제거
        }
        followscene_client_to_server(); //팔로워 가져오기
    }

    //팔로워 가져오기
    public void followscene_client_to_server()
    {
        Followscene_client_to_server follow_scene = new Followscene_client_to_server();
        follow_scene.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow_scene)); //현재 사용자 uid 클라이언트에서 서버로 전달
        Followscene_server_to_client followers = JsonUtility.FromJson<Followscene_server_to_client>(socketpp.receiveMsg);//서버에서 전달받은 것을 클라이언트로 전달

        //이미지 프로필 가져오기
        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        int[] uid_list = new int[followers.follower.Length];
        for (int i = 0; i < uid_list.Length; i++)
        {
            uid_list[i] = followers.follower[i].uid;
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
            MakeFollower(followers.follower[i].uid, followers.follower[i].nickname, followers.follower[i].isfollow, timedict[followers.follower[i].uid], sizedict[followers.follower[i].uid]); //팔로워 정보로 팔로워 Prefab생성
        }
    }

    //팔로워 Prefab생성
    public void MakeFollower(int follower_uid, string follower_nickname, string follower_isfollow, string follower_profile_timestamp, int follower_profile_size)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + follower_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + follower_uid.ToString() + "/"); //플레이어uid폴더 없을 시 생성
        GameObject clone_follower_friend = Instantiate(friend) as GameObject;
        clone_follower_friend.transform.SetParent(this.transform);
        clone_follower_friend.transform.localPosition = Vector3.zero;
        clone_follower_friend.transform.localScale = Vector3.one;
        clone_follower_friend.GetComponent<PrefabUid>().uid = follower_uid; //팔로워 uid
        clone_follower_friend.GetComponent<PrefabUid>().nickname = follower_nickname; //팔로워 닉네임
        GameObject clone_follower_friend_button = clone_follower_friend.transform.Find("FollowerPage").gameObject;
        clone_follower_friend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ActiveFriendPage()); //팔로워 페이지 들어가기 버튼 추가
        GameObject clone_follower_friend_addbutton = clone_follower_friend.transform.Find("addButton").gameObject;
        if(follower_isfollow == "True")
        {
            clone_follower_friend_addbutton.SetActive(false); //팔로우 상태면 팔로우 추가하기 버튼 끄기
        }
        else if(follower_isfollow == "False")
        {
            clone_follower_friend_addbutton.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Friend").GetComponent<FriendScript>().followadd_client_to_server()); //팔로우 상태가 아니면 팔로우 추가하기 버튼 켜기
        }
        GameObject clone_follower_text = clone_follower_friend.transform.Find("FollowerName").gameObject;
        clone_follower_text.GetComponent<Text>().text = follower_nickname; //팔로워 닉네임 텍스트로 보여주기

        //팔로워 프로필 이미지 불러오기
        string path = follower_uid.ToString() + "/" + follower_uid.ToString() + "_" + follower_profile_timestamp + ".png";

        GameObject clone_follower_image = clone_follower_friend.transform.Find("FollowerImage").gameObject;
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_follower_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            iq.size = follower_profile_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            while(true)
            {
                FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path);
                if (info.Length == follower_profile_size)
                {
                    clone_follower_image.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
                    break;
                }
            }
        }
    }
}