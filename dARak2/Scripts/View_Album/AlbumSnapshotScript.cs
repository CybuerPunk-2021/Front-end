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
    //켜질시
    void OnEnable()
    {
        //로그인 사용자 uid와 현재 선택된 스냅샷 uid같으면 3D 에디터 버튼 보이기, 다르면 안보이기
        if (socketpp.player_uid == socketpp.other_player_uid)
        {
            _3DEditorButton.SetActive(true);
        }
        else
        {
            _3DEditorButton.SetActive(false);
        }
        album_snapshot_client_to_server(); //앨범 스냅샷 불러오기
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //현재 선택된 앨범 스냅샷 정보 불러오기
    public void album_snapshot_client_to_server()
    {
        SnapshotWriter.GetComponent<Text>().text = socketpp.other_nickname;
        SnapshotDate.GetComponent<Text>().text = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ParseDateTime(socketpp.snapshot_timestamp);
        SnapshotText.GetComponent<Text>().text = socketpp.snapshot_intro; //스냅샷 설명
        SnapshotLike.GetComponent<Text>().text = socketpp.snapshot_like.ToString(); //스냅샷 좋아요 정보
        SnapshotImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + socketpp.snapshot_timestamp + ".png"); //스냅샷 이미지
    }
}
