using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FriendFriendScript : MonoBehaviour
{
    public GameObject Follower;
    public GameObject Following;
    public Slider friendSlider;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
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
