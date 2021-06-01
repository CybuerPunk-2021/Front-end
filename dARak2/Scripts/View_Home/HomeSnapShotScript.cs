using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeSnapShotScript : MonoBehaviour
{
    public GameObject SnapshotImage, ProfileName, ProfileImage, ProfileText, ProfileLike;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        snapshot_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void snapshot_client_to_server()
    {
        //Profile_client_to_server profile = new Profile_client_to_server();
        //profile.uid = socketpp.player_uid;
        //socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profile));
        //Profile_server_to_client myProfile = JsonUtility.FromJson<Profile_server_to_client>(socketpp.receiveMsg);
        //이미지 관련 추가 필요
        ProfileName.GetComponent<Text>().text = socketpp.other_nickname;
        ProfileText.GetComponent<Text>().text = socketpp.snapshot_intro;
        ProfileLike.GetComponent<Text>().text = socketpp.snapshot_like.ToString();
    }

    public void ARRealBtn()
    {
        GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveARRealScene();
    }
    public void ARMiniBtn()
    {
        GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveARMiniScene();
    }

}
