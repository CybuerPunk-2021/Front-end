using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HomeSnapShotScript : MonoBehaviour
{
    public GameObject SnapshotImage, ProfileName, ProfileImage, ProfileText, ProfileLike;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓 불러오기
        snapshot_client_to_server(); //스냅샷 정보 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //켜질시
    void OnEnable()
    {
        snapshot_client_to_server(); //스냅샷 정보 가져오기
    }

    public void snapshot_client_to_server()
    {
        //스냅샷 정보
        ProfileName.GetComponent<Text>().text = socketpp.other_nickname;
        ProfileText.GetComponent<Text>().text = socketpp.snapshot_intro;
        ProfileLike.GetComponent<Text>().text = socketpp.snapshot_like.ToString();

        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = new int[1] { socketpp.other_player_uid };
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile)); //다른 사용자 uid 클라이언트에서 서버로 전달
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달

        //프로필 이미지, 스냅샷 이미지
        ProfileImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + checkprofiletime.timestamp[0] + ".png");
        SnapshotImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + socketpp.snapshot_timestamp + ".png");
    }
}
