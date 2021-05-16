using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FriendBtn : MonoBehaviour
{
    public GameObject Follower;
    public GameObject Following;
    public Slider friendSlider;
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
