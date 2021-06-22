using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;

public class SearchScript : MonoBehaviour
{
    public GameObject searchFriend;
    public InputField searchInputField;
    public GameObject popup;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓불러오기
    }

    //팔로우 버튼 누를 시
    public void FollowBtn()
    {
        follow_client_to_server(); //팔로우 정보 가져오기
        EraseSearch(); //검색 Prefab삭제
        search_client_to_server(); //검색하기
        
    }

    //검색 버튼 누를 시
    public void SearchBtn()
    {
        EraseSearch(); //검색 Prefab삭제
        search_client_to_server(); //검색하기
    }

    //검색 Prefab삭제
    public void EraseSearch()
    {
        GameObject[] searchedFriends = GameObject.FindGameObjectsWithTag("SearchFriend");
        foreach (GameObject searchedFriend in searchedFriends)
        {
            Destroy(searchedFriend); //searchFriend prefab삭제
        }
    }
    
    //팔로우 하기
    public void follow_client_to_server()
    {
        Followadd_client_to_server follow = new Followadd_client_to_server();
        follow.from_uid = socketpp.player_uid;
        GameObject current = EventSystem.current.currentSelectedGameObject;
        follow.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow)); //현재 사용자 uid와 대상 uid를 클라이언트에서 서보로 전달
        popup.transform.GetChild(0).GetComponent<Text>().text = "팔로우 완료";
        popup.SetActive(true);
    }

    //검색하기
    public void search_client_to_server()
    {
        Search_client_to_server search = new Search_client_to_server();
        search.query = searchInputField.text;
        search.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(search)); //검색할 사람이름 텍스트 필드, 사용자 uid 클라이언트에서 서버로 전달
        Search_server_to_client search_result = JsonUtility.FromJson<Search_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        
        //이미지 프로필 가져오기
        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        int[] uid_list = new int[search_result.result.Length];
        for (int i = 0; i < uid_list.Length; i++)
        {
            uid_list[i] = search_result.result[i].uid;
        }
        checkprofile.uid = uid_list.Distinct().ToArray();
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);
        Dictionary<int, int> sizedict = new Dictionary<int, int>();
        Dictionary<int, string> timedict = new Dictionary<int, string>();

        for (int i = 0; i < checkprofile.uid.Length; i++)
        {
            timedict.Add(checkprofile.uid[i], checkprofiletime.timestamp[i]);
            sizedict.Add(checkprofile.uid[i], checkprofiletime.size[i]);
        }

        for (int i=0; i<50; i++)
        {
            Make_Search_Clone(search_result.result[i].uid, search_result.result[i].nickname, search_result.result[i].isfollow, timedict[search_result.result[i].uid], sizedict[search_result.result[i].uid]); //검색된 사용자 정보로 검색 사용자 Prefab생성
        }
    }

    //검색 사용자 Prefab생성
    public void Make_Search_Clone(int result_uid, string result_nickname, string result_isfollow, string result_profile_timestamp, int result_profile_size)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + result_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + result_uid.ToString() + "/"); //플레이어uid폴더 없을 시 생성
        GameObject clone_searchFriend = Instantiate(searchFriend) as GameObject;
        clone_searchFriend.transform.SetParent(this.transform);
        clone_searchFriend.transform.localPosition = Vector3.zero;
        clone_searchFriend.transform.localScale = Vector3.one;
        clone_searchFriend.GetComponent<PrefabUid>().uid = result_uid; //검색된 사람 uid
        clone_searchFriend.GetComponent<PrefabUid>().nickname = result_nickname; //검색된 사람 닉네임
        GameObject clone_searchFriend_button = clone_searchFriend.transform.Find("SearchFriendPage").gameObject;
        clone_searchFriend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ActiveFriendPage()); //검색된 사람 페이지 들어가기 버튼 추가
        GameObject clone_searchFriend_Follow_button = clone_searchFriend.transform.Find("addButton").gameObject;
        if (result_isfollow == "True") //팔로우 상태면 팔로우 버튼 끄기, 팔로우 상태 아니면 팔로우 버튼 켜기
        {
            clone_searchFriend_Follow_button.SetActive(false);
        }
        else if (result_isfollow == "False")
        {
            clone_searchFriend_Follow_button.GetComponent<Button>().onClick.AddListener(() => FollowBtn());
        }

        //검색된 사람 프로필 이미지 불러오기
        string path = result_uid.ToString() + "/" + result_uid.ToString() + "_" + result_profile_timestamp + ".png";

        GameObject clone_searchFriend_image = clone_searchFriend.transform.Find("SearchFriendImage").gameObject;
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_searchFriend_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            iq.size = result_profile_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            while (true)
            {
                FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path);
                if (info.Length == result_profile_size)
                {
                    clone_searchFriend_image.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
                    break;
                }
            }
        }

        GameObject clone_searchFriend_text = clone_searchFriend.transform.Find("SearchFriendName").gameObject;
        clone_searchFriend_text.GetComponent<Text>().text = result_nickname;
    }
}
