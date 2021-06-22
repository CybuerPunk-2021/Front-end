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
    public GameObject popup;
    Socketpp socketpp;
    public void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓 불러오기
    }
    //팔로우추가버튼 누를시 팔로우 추가
    public void followadd_client_to_server()
    {
        Followadd_client_to_server follow = new Followadd_client_to_server();
        GameObject current = EventSystem.current.currentSelectedGameObject;
        follow.from_uid = socketpp.player_uid;
        follow.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(follow)); //팔로우 사용자 uid 팔로우 대상 uid 서버에 전달
        Followadd_server_to_client follow_result = JsonUtility.FromJson<Followadd_server_to_client>(socketpp.receiveMsg);//서버에서 받은 값 클라이언트에 전달
        if(follow_result.action == "OK") //팔로우 ok면
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "팔로우 완료";
            popup.SetActive(true);
            GameObject.Find("View_Follower_Content").GetComponent<FollowerScript>().UpdateFollower();
        }
        else if(follow_result.action == "ALREADY") //팔로우를 이미 한 상태면
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "팔로우를 삭제했습니다";
            popup.SetActive(true);
        }
    }

    //팔로우삭제버튼 누를시 팔로우 삭제
    public void followdelete_client_to_server()
    {
        Followdelete_client_to_server unfollow = new Followdelete_client_to_server();
        GameObject current = EventSystem.current.currentSelectedGameObject;
        unfollow.from_uid = socketpp.player_uid;
        unfollow.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(unfollow)); //팔로우 사용자 uid 팔로우 대상 uid 서버에 전달
        Followdelete_server_to_client unfollow_result = JsonUtility.FromJson<Followdelete_server_to_client>(socketpp.receiveMsg);//서버에서 받은 값 클라이언트에 전달
        if (unfollow_result.action == "OK") //언팔로우 ok면
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "팔로우 삭제 완료";
            popup.SetActive(true);
            GameObject.Find("View_Following_Content").GetComponent<FollowingScript>().UpdateFollowing();
        }
        else if (unfollow_result.action == "ALREADY") //언팔로우 이미 한 상태면
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "이미 팔로우가 삭제되었습니다";
            popup.SetActive(true);
        }
    }

    //팔로워 버튼 클릭 시
    public void ActiveFollower()
    {
        friendSlider.value = 0;
        Following.SetActive(false);
        Follower.SetActive(true);
    }

    //팔로잉 버튼 클릭 시
    public void ActiveFollowing()
    {
        friendSlider.value = 1;
        Following.SetActive(true);
        Follower.SetActive(false);
    }
}
