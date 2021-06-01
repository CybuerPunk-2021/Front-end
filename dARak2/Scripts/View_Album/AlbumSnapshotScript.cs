using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumSnapshotScript : MonoBehaviour
{
    public GameObject SnapshotImage, SnapshotDate, SnapshotText, SnapshotLike;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        album_snapshot_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void album_snapshot_client_to_server()
    {
        //스냅샷 이미지 추가 필요
        SnapshotDate.GetComponent<Text>().text = socketpp.snapshot_timestamp;
        SnapshotText.GetComponent<Text>().text = socketpp.snapshot_intro;
        SnapshotLike.GetComponent<Text>().text = socketpp.snapshot_like.ToString();
    }
}
