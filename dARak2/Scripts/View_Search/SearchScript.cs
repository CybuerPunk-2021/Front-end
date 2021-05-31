using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SearchScript : MonoBehaviour
{
    public GameObject searchFriend;
    public InputField searchInputField;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }

    // Update is called once per frame
    void Update()
    {
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
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(search));
        Search_server_to_client search_result = JsonUtility.FromJson<Search_server_to_client>(socketpp.receiveMsg);
        for(int i=0; i<50; i++)
        {
            Make_Search_Clone(search_result.result[i].uid, search_result.result[i].nickname);
        }
    }

    public void Make_Search_Clone(int result_uid, string result_nickname)
    {
        GameObject clone_searchFriend = Instantiate(searchFriend) as GameObject;
        clone_searchFriend.transform.SetParent(this.transform);
        clone_searchFriend.transform.localPosition = Vector3.zero;
        clone_searchFriend.transform.localScale = Vector3.one;
        clone_searchFriend.GetComponent<PrefabUid>().uid = result_uid;
        clone_searchFriend.GetComponent<PrefabUid>().nickname = result_nickname;
        GameObject clone_searchFriend_button = clone_searchFriend.transform.Find("SearchFriendPage").gameObject;
        clone_searchFriend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        GameObject clone_searchFriend_Follow_button = clone_searchFriend.transform.Find("addButton").gameObject;
        clone_searchFriend_Follow_button.GetComponent<Button>().onClick.AddListener(() => follow_client_to_server());
        //GameObject searchFriend_image = clone_snapshot.transform.Find("SearchFriendImage").gameObject;
        //searchFriend_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_searchFriend_text = clone_searchFriend.transform.Find("SearchFriendName").gameObject;
        clone_searchFriend_text.GetComponent<Text>().text = result_nickname;
    }
}
