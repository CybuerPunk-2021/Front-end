using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class FriendFollowerScript : MonoBehaviour
{
    public GameObject friend;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }
    void OnEnable()
    {
        UpdateFollower();
    }

    public void UpdateFollower()
    {
        GameObject[] followers = GameObject.FindGameObjectsWithTag("FriendFollower");
        foreach (GameObject follower in followers)
        {
            Destroy(follower);
        }
        followscene_client_to_server();
    }

    public void followscene_client_to_server()
    {
        Followscene_client_to_server follow_scene = new Followscene_client_to_server();
        follow_scene.uid = socketpp.other_player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow_scene));
        Followscene_server_to_client followers = JsonUtility.FromJson<Followscene_server_to_client>(socketpp.receiveMsg);
        for (int i = 0; i < 50; i++)
        {
            MakeFollower(followers.follower[i].uid, followers.follower[i].nickname);
        }
    }

    public void MakeFollower(int follower_uid, string follower_nickname)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + follower_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + follower_uid.ToString() + "/");
        GameObject clone_follower_friend = Instantiate(friend) as GameObject;
        clone_follower_friend.transform.SetParent(this.transform);
        clone_follower_friend.transform.localPosition = Vector3.zero;
        clone_follower_friend.transform.localScale = Vector3.one;
        clone_follower_friend.GetComponent<PrefabUid>().uid = follower_uid;
        clone_follower_friend.GetComponent<PrefabUid>().nickname = follower_nickname;
        GameObject clone_follower_friend_button = clone_follower_friend.transform.Find("FollowerPage").gameObject;
        clone_follower_friend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        GameObject clone_follower_friend_addbutton = clone_follower_friend.transform.Find("addButton").gameObject;
        clone_follower_friend_addbutton.SetActive(false);
        GameObject clone_follower_text = clone_follower_friend.transform.Find("FollowerName").gameObject;
        clone_follower_text.GetComponent<Text>().text = follower_nickname;

        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = follower_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);

        string path = follower_uid.ToString() + "/" + follower_uid.ToString() + "_" + checkprofiletime.timestamp + ".png";
        GameObject clone_follower_image = clone_follower_friend.transform.Find("FollowerImage").gameObject;
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_follower_image.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            clone_follower_image.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
        }
    }
}
