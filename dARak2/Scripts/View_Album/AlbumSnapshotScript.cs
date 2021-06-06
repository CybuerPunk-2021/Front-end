using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AlbumSnapshotScript : MonoBehaviour
{
    public GameObject SnapshotWriter, SnapshotImage, SnapshotDate, SnapshotText, SnapshotLike, _3DEditorButton;
    Socketpp socketpp;
    // Start is called before the first frame update

    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }
    void OnEnable()
    {
        if (socketpp.player_uid == socketpp.other_player_uid)
        {
            _3DEditorButton.SetActive(true);
        }
        else
        {
            _3DEditorButton.SetActive(false);
        }
        album_snapshot_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void album_snapshot_client_to_server()
    {
        SnapshotWriter.GetComponent<Text>().text = socketpp.other_nickname;
        SnapshotDate.GetComponent<Text>().text = GameObject.Find("View_Main").GetComponent<MainSceneScript>().ParseDateTime(socketpp.snapshot_timestamp);
        SnapshotText.GetComponent<Text>().text = socketpp.snapshot_intro;
        SnapshotLike.GetComponent<Text>().text = socketpp.snapshot_like.ToString();
        SnapshotImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + socketpp.snapshot_timestamp + ".png");
    }
}
