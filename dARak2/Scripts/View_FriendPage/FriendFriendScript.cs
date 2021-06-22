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
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓 불러오기
    }

    //친구의 친구페이지 팔로워 버튼 클릭 시
    public void ActiveFollower()
    {
        friendSlider.value = 0;
        Following.SetActive(false);
        Follower.SetActive(true);
    }

    //친구의 친구페이지 팔로잉 버튼 클릭 시
    public void ActiveFollowing()
    {
        friendSlider.value = 1;
        Following.SetActive(true);
        Follower.SetActive(false);
    }
}
