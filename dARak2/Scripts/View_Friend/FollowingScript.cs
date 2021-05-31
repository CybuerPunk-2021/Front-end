using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowingScript : MonoBehaviour
{
    public GameObject friend;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        followingscene_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateFollowing()
    {
        GameObject[] followings = GameObject.FindGameObjectsWithTag("Following");
        foreach (GameObject following in followings)
        {
            Destroy(following);
        }
        followingscene_client_to_server();
    }

    public void followingscene_client_to_server()
    {
        Followingscene_client_to_server following_scene = new Followingscene_client_to_server();
        following_scene.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(following_scene));
        Followingscene_server_to_client followings = JsonUtility.FromJson<Followingscene_server_to_client>(socketpp.receiveMsg);
        for(int i = 0; i < 50; i++)
        {
            MakeFollowing(followings.following[i].uid, followings.following[i].nickname);
        }
    }

    public void MakeFollowing(int following_uid, string following_nickname)
    {
        GameObject clone_following_friend = Instantiate(friend) as GameObject;
        clone_following_friend.transform.SetParent(this.transform);
        clone_following_friend.transform.localPosition = Vector3.zero;
        clone_following_friend.transform.localScale = Vector3.one;
        clone_following_friend.GetComponent<PrefabUid>().uid = following_uid;
        clone_following_friend.GetComponent<PrefabUid>().nickname = following_nickname;
        GameObject clone_following_friend_button = clone_following_friend.transform.Find("FollowingPage").gameObject;
        clone_following_friend_button.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        GameObject clone_following_friend_deletebutton = clone_following_friend.transform.Find("deleteButton").gameObject;
        clone_following_friend_deletebutton.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Friend").GetComponent<FriendScript>().followdelete_client_to_server());
        GameObject clone_following_text = clone_following_friend.transform.Find("FollowingName").gameObject;
        clone_following_text.GetComponent<Text>().text = following_nickname;
        //GameObject clone_following_image = clone_snapshot.transform.Find("FollowingImage").gameObject;
        //clone_following_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
    }
}