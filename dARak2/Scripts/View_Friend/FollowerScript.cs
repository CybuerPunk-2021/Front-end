using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowerScript : MonoBehaviour
{
    public GameObject friend;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        followscene_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateFollower()
    {
        GameObject[] followers = GameObject.FindGameObjectsWithTag("Follower");
        foreach (GameObject follower in followers)
        {
            Destroy(follower);
        }
        followscene_client_to_server();
    }

    public void followscene_client_to_server()
    {
        Followscene_client_to_server follow_scene = new Followscene_client_to_server();
        follow_scene.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow_scene));
        Followscene_server_to_client followers = JsonUtility.FromJson<Followscene_server_to_client>(socketpp.receiveMsg);
        for(int i = 0; i < 50; i++)
        {
            MakeFollower(followers.follower[i].uid, followers.follower[i].nickname);
        }
    }

    public void MakeFollower(int follower_uid, string follower_nickname)
    {
        GameObject clone_follower_friend = Instantiate(friend) as GameObject;
        clone_follower_friend.transform.SetParent(this.transform);
        clone_follower_friend.transform.localPosition = Vector3.zero;
        clone_follower_friend.transform.localScale = Vector3.one;
        clone_follower_friend.GetComponent<PrefabUid>().uid = follower_uid;
        clone_follower_friend.GetComponent<PrefabUid>().nickname = follower_nickname;
        GameObject clone_follower_friend_button = clone_follower_friend.transform.Find("FollowerPage").gameObject;
        clone_follower_friend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        GameObject clone_follower_friend_addbutton = clone_follower_friend.transform.Find("addButton").gameObject;
        clone_follower_friend_addbutton.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Friend").GetComponent<FriendScript>().followadd_client_to_server());
        //GameObject clone_follower_image = clone_snapshot.transform.Find("FollowerImage").gameObject;
        //clone_follower_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_follower_text = clone_follower_friend.transform.Find("FollowerName").gameObject;
        clone_follower_text.GetComponent<Text>().text = follower_nickname;
    }
}