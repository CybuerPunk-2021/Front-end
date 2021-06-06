using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
public class SearchScript : MonoBehaviour
{
    public GameObject searchFriend;
    public InputField searchInputField;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }

    public void FollowBtn()
    {
        follow_client_to_server();
        EraseSearch();
        search_client_to_server();
    }

    public void SearchBtn()
    {
        EraseSearch();
        search_client_to_server();
    }

    public void EraseSearch()
    {
        GameObject[] searchedFriends = GameObject.FindGameObjectsWithTag("SearchFriend");
        foreach (GameObject searchedFriend in searchedFriends)
        {
            Destroy(searchedFriend);
        }
    }

    public void follow_client_to_server()
    {
        Followadd_client_to_server follow = new Followadd_client_to_server();
        follow.from_uid = socketpp.player_uid;
        GameObject current = EventSystem.current.currentSelectedGameObject;
        follow.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow));
    }

    public void search_client_to_server()
    {
        Search_client_to_server search = new Search_client_to_server();
        search.query = searchInputField.text;
        search.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(search));
        Search_server_to_client search_result = JsonUtility.FromJson<Search_server_to_client>(socketpp.receiveMsg);
        for(int i=0; i<50; i++)
        {
            Make_Search_Clone(search_result.result[i].uid, search_result.result[i].nickname, search_result.result[i].isfollow);
        }
    }

    public void Make_Search_Clone(int result_uid, string result_nickname, string result_isfollow)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + result_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + result_uid.ToString() + "/");
        GameObject clone_searchFriend = Instantiate(searchFriend) as GameObject;
        clone_searchFriend.transform.SetParent(this.transform);
        clone_searchFriend.transform.localPosition = Vector3.zero;
        clone_searchFriend.transform.localScale = Vector3.one;
        clone_searchFriend.GetComponent<PrefabUid>().uid = result_uid;
        clone_searchFriend.GetComponent<PrefabUid>().nickname = result_nickname;
        GameObject clone_searchFriend_button = clone_searchFriend.transform.Find("SearchFriendPage").gameObject;
        clone_searchFriend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        GameObject clone_searchFriend_Follow_button = clone_searchFriend.transform.Find("addButton").gameObject;
        if (result_isfollow == "True")
        {
            clone_searchFriend_Follow_button.SetActive(false);
        }
        else if (result_isfollow == "False")
        {
            clone_searchFriend_Follow_button.GetComponent<Button>().onClick.AddListener(() => FollowBtn());
        }

        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = result_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);

        string path = result_uid.ToString() + "/" + result_uid.ToString() + "_" + checkprofiletime.timestamp + ".png";

        GameObject clone_searchFriend_image = clone_searchFriend.transform.Find("SearchFriendImage").gameObject;
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_searchFriend_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            clone_searchFriend_image.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
        }

        GameObject clone_searchFriend_text = clone_searchFriend.transform.Find("SearchFriendName").gameObject;
        clone_searchFriend_text.GetComponent<Text>().text = result_nickname;
    }
}
