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

    void OnEnable()
    {
        snapshot_client_to_server();
    }

    public void snapshot_client_to_server()
    {
        ProfileName.GetComponent<Text>().text = socketpp.other_nickname;
        ProfileText.GetComponent<Text>().text = socketpp.snapshot_intro;
        ProfileLike.GetComponent<Text>().text = socketpp.snapshot_like.ToString();

        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        checkprofile.uid = socketpp.other_player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);
        ProfileImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + checkprofiletime.timestamp + ".png");
        SnapshotImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + socketpp.snapshot_timestamp + ".png");
    }
}
