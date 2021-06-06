using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class FriendScript : MonoBehaviour
{
    public GameObject Follower;
    public GameObject Following;
    public Slider friendSlider;
    Socketpp socketpp;
    public void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }
    public void followadd_client_to_server()
    {
        Followadd_client_to_server follow = new Followadd_client_to_server();
        GameObject current = EventSystem.current.currentSelectedGameObject;
        follow.from_uid = socketpp.player_uid;
        follow.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow));
        GameObject.Find("View_Follower_Content").GetComponent<FollowerScript>().UpdateFollower();
    }

    public void followdelete_client_to_server()
    {
        Followdelete_client_to_server unfollow = new Followdelete_client_to_server();
        GameObject current = EventSystem.current.currentSelectedGameObject;
        unfollow.from_uid = socketpp.player_uid;
        unfollow.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(unfollow));
        GameObject.Find("View_Following_Content").GetComponent<FollowingScript>().UpdateFollowing();
    }

    public void ActiveFollower()
    {
        friendSlider.value = 0;
        Following.SetActive(false);
        Follower.SetActive(true);
    }

    public void ActiveFollowing()
    {
        friendSlider.value = 1;
        Following.SetActive(true);
        Follower.SetActive(false);
    }
}
